<?php
namespace Napkins;

require_once(__DIR__ . "/class_template.php");

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Jenssegers\ImageHash\ImageHash;
use Imagick;

Class ImageController {
private $errors;
public function imageView(Request $request, Application $app) {
	if(null == $email = $app["session"]->get("email"))
	{
		$app["session"]->set("dir", "images");
		return $app->redirect("login");
	}
	$imageDir = $app["imageDirBase"].$email;
	$thumbnailExtension = $app["imageThumbnailExtention"];

	$sql = "SELECT * FROM images WHERE email = :email";
	$foundSQL = "SELECT * FROM found WHERE hash = :hash";
	$grades = ['A', 'B', 'C', 'D'];
	$templating = new WebTemplate();
	$hasher = new ImageHash(NULL, ImageHash::DECIMAL);

	$userImages = $app["db"]->fetchAll($sql, Array("email" => $email));
	for($i = 0; $i < count($userImages); ++$i) {
		$imageFile = $imageDir."/".$userImages[$i]["hash"].$thumbnailExtension.".".$userImages[$i]["extension"];
		$userImages[$i]["imageFile"] = base64_encode(file_get_contents($imageFile));
		if($userImages[$i]["imageFile"] == False) {
			array_push($this->errors, "Image file ".$userImages[$i]["orig_name"]." was not found");
		}
		$userImages[$i]["found"] = $app["db"]->fetchAll($foundSQL,["hash" => $userImages[$i]["hash"]]);
		for($j = 0; $j < count($userImages[$i]["found"]); ++$j) {
			$userImages[$i]["found"][$j]["grade"] = $hasher->distance($userImages[$i]['hash'], $userImages[$i]["found"][$j]['hash']);
		}
		//		var_dump($imageFile);
	}
	//	var_dump($userImages);
	$return = ["images"=>$userImages];
	$page_content = $templating->render("image_list.php", $return);

	$templating->setTitle("Upload Image");
	$templating->addGlobal("page_content", $page_content);
	$templating->addGlobal("login", TRUE);
	//	$retErr = [];
	//	if(isSet($this->errors)) {
	//		$retErr = $this->errors;
	//		$this->errors = [];
	//	}
	return $templating->renderDefault($this->errors);
}
public function processImages(Request $request, Application $app) {
	if(null == $email = $app["session"]->get("email"))
	{
		$app["session"]->set("dir", "import");
		return $app->redirect("images");
	}
	$data = $request->request->all();
	$this->errors = [];
	//	var_dump($data);
	foreach($data as $key => $value) {
		if($value == "delete") {

			array_push($this->errors,$this->deleteImage($email, $key, $app));
		} else if($value == "merge") {

			$sqlFinger = "SELECT fingerprint FROM fingerprints WHERE email = :email";
			$mark = $app["db"]->fetchAll($sqlFinger, ["email" => $email]);

			if(count($mark) == 0) {
				array_push($this->errors, "No Mark found");
			} else {
				$this->mergeFingerprint($app, $key, $mark[0]["fingerprint"], $email);
			}
		}
	}



	return $this->imageView($request, $app);
}

function mergeFingerprint($app, $hash, $mark, $email){
	$imageDir = $app["imageDirBase"].$email;
	$thumbnailExtension = $app["imageThumbnailExtention"];
	$sql = "SELECT * FROM images WHERE hash = :hash AND email = :email";
	$res = $app["db"]->fetchAll($sql, ["email" => $email, "hash" => $hash]);
	$ext = $res[0]["extension"];
	$name = $res[0]["orig_name"];
	$image = $imageDir."/".$hash.".".$ext;
	$markfile = $imageDir."/fingerprints/".$mark;

	//take $image image and merge with fingerprint $fingerP

	switch ($ext) {
	case 'jpg':
	case 'jpeg':
		$img = @imagecreatefromjpeg($image);
		break;
	case 'gif':
		$img = @imagecreatefromgif($image);
		break;
	case 'png':
		$img = @imagecreatefrompng($image);
		break;
	default:
		return false;
	}
	$hasher = new ImageHash(NULL, ImageHash::DECIMAL);
	$fingerprint = @imagecreatefrompng($markfile);

	$width = imagesx($img);
	$height = imagesy($img);
	$fingerprint = imagescale($fingerprint,$width,$height);
	imagecopymerge($img, $fingerprint, 0, 0, 0, 0, $width, $height, 20); 
	$hash = $hasher->hash($img);
	imagepng($img, $imageDir."/".$hash.".".$ext);
	header('Content-type: image/png');
//	imagepng($img);
	$this->createThumbnail($imageDir."/".$hash.".".$ext, $imageDir."/".$hash.$thumbnailExtension.".".$ext);

	//add name to database images (hash, email, image, date)
	$sql = "INSERT INTO images (hash, email, date, orig_name, extension, min_grade) VALUES (:hash, :email, :date, :name, :ext, 'C')";
	$stmt = $app["db"]->prepare($sql);
	$stmt->bindValue("hash", $hash);
	$stmt->bindValue("email", $email);
	$stmt->bindValue("date", date("y-m-d H:i"));
	$stmt->bindValue("name", "fingerprint ".$mark." ".$name);
	$stmt->bindValue("ext", $ext);
	if(!$stmt->execute()){
		throw new \Exception("Failed to merge image with fingerprint");
	}

}

private function createThumbnail($imageFile, $thumbNailFile) {
	$image = new \Imagick(realpath($imageFile));
	$image->setbackgroundcolor('rgb(64,64,64)');
	$image->thumbnailImage(200,200,true);
	$image->writeImage($thumbNailFile);
}

public function deleteImage($email, $hash, $app) {
	$imageDir = $app["imageDirBase"].$email;
	$thumbnailExtension = $app["imageThumbnailExtention"];

	$sql = "SELECT * FROM images WHERE hash = :hash AND email = :email";
	$image = $app["db"]->fetchAll($sql, ["email" => $email, "hash" => $hash]);
	try {
		if(count($image) != 1) {
			throw new \Exception("Image not found");
		}
		$sqlDeleteImage = "DELETE FROM images WHERE hash = :hash AND email = :email";

		$stmt = $app["db"]->prepare($sqlDeleteImage);
		$stmt->bindValue("hash", $hash);
		$stmt->bindValue("email", $email);
		//need to delete image
		if(!$stmt->execute())
		{
			throw new \Exception("Image ".$image[0]["orig_name"]." could not be deleted from database");
		}
		if(!unlink($imageDir."/".$hash.".".$image[0]["extension"])) {
			throw new \Exception("Image ".$image[0]["orig_name"]." file could not be deleted");
		}
		if(!unlink($imageDir."/".$hash.$thumbnailExtension.".".$image[0]["extension"])) {
			throw new \Exception("Image ".$image[0]["orig_name"]." file could not be deleted");
		}
	} catch(\Exception $e) {
		return $e->getMessage();
	}
}
public function notifyFound(Request $request, Application $app) {
	if(null == $email = $app["session"]->get("email"))
	{
		$app["session"]->set("dir", "import");
		return $app->redirect("notify");
	}
	$url = $request->query->get("Addr");
	$parsedUrl = parse_url($url);
	$sql = "SELECT * FROM crawler WHERE config_key LIKE 'url' AND config_value LIKE :host";
	$entry = $app["db"]->fetchAll($sql, ["host" => $parsedUrl["host"]]);
	$emailAddr = '';
	if(count($entry) <= 0) {
		$allInfo = explode("\n",shell_exec("whois ".$parsedUrl["host"]));
		$infoList = [];
		foreach($allInfo as $value) {
			$lineInfo = explode(":", $value, 2);
			if(count($lineInfo) == 2) {
				$infoList[$lineInfo[0]] = $lineInfo[1];
			}
		}
		if(isset($infoList["Admin Email"])&&strlen($infoList["Admin Email"]) > 0) {
			$emailAddr = $infoList["Admin Email"];
			break;
		}
		if(isset($infoList["Tech Email"])&&strlen($infoList["Tech Email"])) {
			$emailAddr = $infoList["Tech Email"];
			break;
		}
		if(isset($infoList["Registrant Email"])&&strlen($infoList["Registrant Email"])) {
			$emailAddr = $infoList["Registrant Email"];
			break;
		}
	} else {
		$emailAddr = $entry[0]["value2"];
	}

	$subject = 'Use of Copywrited Image';
    $headers = 'From: Auld Corp <do-not-reply@auldcorporation.com>' . "\r\n" .
        'To: '.$emailAddr."\r\n" .
		'X-Mailer: PHP/' . phpversion();
	$message = "To whom it may concern:"."\r\n".
		"\r\n".
        "We have found a copywrited image belonging to one of our users on your"."\r\n".
		"website. Please remove the following image:\r\n".
		"\r\n".
		"Domain: ".$parsedUrl["host"]."\r\n".
		"\r\n".
		"Image Url: ".$url."\r\n".
		"\r\n".
		"Best regards,"."\r\n".
		"The Auld Corporation"."\r\n";

	mail($emailAddr, $subject, $message, $headers);

	return $this->imageView($request, $app);
}
}
?>
