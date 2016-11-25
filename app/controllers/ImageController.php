<?php
namespace Napkins;

require_once(__DIR__ . "/class_template.php");

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Jenssegers\ImageHash\ImageHash;

Class ImageController {
private $errors = [];
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
		}
	}
	return $this->imageView($request, $app);
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
	$sql = "SELECT * FROM crawler WHERE config_key = 'url' AND config_value = :host";
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
		if(isset($infoList["Registrant Email"])&&strlen($infoList["Registrant Email"])) {
			$emailAddr = $infoList["Registrant Email"];
			break;
		}
		if(isset($infoList["Tech Email"])&&strlen($infoList["Tech Email"])) {
			$emailAddr = $infoList["Tech Email"];
			break;
		}
	} else {
		$emailAddr = $entry[0]["value2"];
	}

	$subject = 'Use of Copywrited Image';
	$headers = 'From: Auld Corp <do-not-reply@auldcorporation.com>' . "\r\n" .
		'X-Mailer: PHP/' . phpversion();
	$message = "To whom it may concern,"."\r\n".
		"\r\n".
		"The service _______ has found a copywrited image belonging to one of our users on your website\r\n".
		"\r\n".
		"Domain: ".$parsedUrl["host"]."\r\n".
		"\r\n".
		"Image Url: ".$url."\r\n".
		"\r\n".
		"Please remove the conflicting image from your site\r\n".
		"\r\n".
		"Best regards,"."\r\n".
		"The Auld Corporation"."\r\n";

	mail($emailAddr, $subject, $message, $headers);

	return $this->imageView($request, $app);
}
}
?>
