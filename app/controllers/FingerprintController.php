<?php
namespace Napkins;

require_once(__DIR__ . "/class_template.php");

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Jenssegers\ImageHash\ImageHash;

Class FingerprintController {
private $errors = [];

public function fingerprintView(Request $request, Application $app) {
	if(null == $email = $app["session"]->get("email"))
	{
		$app["session"]->set("dir", "fingerprints");
		return $app->redirect("login");
	}
	$fingerprintDir = $app["imageDirBase"].$email."/fingerprints";

	$sql = "SELECT * FROM fingerprints WHERE email = :email";
	$templating = new WebTemplate();

	$userImages = $app["db"]->fetchAll($sql, Array("email" => $email));
	for($i = 0; $i < count($userImages); ++$i) {
		$imageFile = $fingerprintDir."/".$userImages[$i]["fingerprint"];
		$userImages[$i]["imageFile"] = base64_encode(file_get_contents($imageFile));
		if($userImages[$i]["imageFile"] == False) {
			array_push($this->errors, "Finger print file ".$userImages[$i]["fingerprint"]." was not found");
		}
	}
	//	var_dump($userImages);
	$return = ["images"=>$userImages];
	$page_content = $templating->render("fingerprint_list.php", $return);

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
//creates a png image with ellipses on it
//TODO this should do other shapes. only ellipses for now for simplicity
function newFingerprint(Request $request, Application $app){
	$imageDir = "";
	$folder = "/fingerprints";
	if (Null == $email = $app["session"]->get("email")) {
		$app["session"]->set("dir", "images");
		return $app->redirect("login");
	}
	$imageDir = $app["imageDirBase"].$email.$folder;

	// integer starts at 0 before counting
	$num = 0;
	if(NULL != $var = $app["db"]->query("SELECT MAX(fingerprint) AS num FROM fingerprints WHERE email='".$email."'")) {
		$num = $var->fetch()["num"] +1;
	}
	//add name to database
	$succeed = False;
	$sql = "INSERT INTO fingerprints (fingerprint, email) VALUES (:i, :email)";
	$stmt = $app["db"]->prepare($sql);
	$stmt->bindValue("i", $num);
	$stmt->bindvalue("email", $email);
	if (!$stmt->execute()){
		throw new \Exception("Failed to create fingerprint");
	}


	$width = 300;
	$height = 300;
	$numShapes = 13;

	header ('Content-Type: image/png');
	$fingerprint = @imagecreatetruecolor($width, $height)
		or die('Cannot create fingerprint');

	$trans = imagecolorallocatealpha($fingerprint, 250, 250, 250, 127);
	imagefill($fingerprint, 0, 0, $trans);

	//loop over  with different shapes and coordinates
	for ($i=0; $i<$numShapes; $i++) {
		$xCoord = rand(1, $width);
		$yCoord = rand(1, $height);

		$shapeWidth = rand(1, 20);
		$shapeHeight = rand(1, 20);
		$color = imagecolorallocate($fingerprint, rand(0,255), rand(0,255), rand(0,255)); //random color

		imagefilledellipse($fingerprint, $xCoord, $yCoord, $shapeWidth, $shapeHeight, $color);
	}
	if(!file_exists($imageDir)) {
		mkdir($imageDir);
	}
	imagepng($fingerprint, $imageDir."/".$num);
	return $app->redirect("fingerprints");
}

//TODO function to apply previously created fingerprints to a given image
function addFingerprint($file, $fingerP){
	$extension = strtolower(strrchr($file, '.'));

	switch ($extension) {
	case '.jpg':
	case '.jpeg':
		$img = @imagecreatefromjpeg($file);
		break;
	case '.gif':
		$img = @imagecreatefromgif($file);
		break;
	case '.png':
		$img = @imagecreatefrompng($file);
		break;
	default:
		return false;
	}

	$fingerprint = @imagecreatefrompng($fingerP);

	imagecopymerge($img, $fingerprint, 0, 0, 0, 0, $width, $height); 
	header('Content-type: image/png');
	imagepng($img);

}

function fingerprintListAction(Request $request, Application $app) {
	if(null == $email = $app["session"]->get("email"))
	{
		$app["session"]->set("dir", "import");
		return $app->redirect("fingerprintss");
	}
	$data = $request->request->all();
	$this->errors = [];
	//	var_dump($data);
	foreach($data as $key => $value) {
		if($value == "delete") {
			$temp = $this->deleteFingerprint($email, $key, $app);
			if(!empty($temp)) {
				array_push($this->errors,$temp);
			}
		}
	}
	return $this->fingerprintView($request, $app);
}

function deleteFingerprint($email, $fingerprint, $app) {
	$imageDir = $app["imageDirBase"].$email;
	$folder = "/fingerprints";

	$sql = "SELECT * FROM fingerprints WHERE fingerprint = :fingerprint AND email = :email";
	$image = $app["db"]->fetchAll($sql, ["email" => $email, "fingerprint" => $fingerprint]);
	try {
		if(count($image) != 1) {
			throw new \Exception("Fingerprint not found");
		}
		$sqlDeleteImage = "DELETE FROM fingerprints WHERE fingerprint = :fingerprint AND email = :email";

		$stmt = $app["db"]->prepare($sqlDeleteImage);
		$stmt->bindValue("fingerprint", $fingerprint);
		$stmt->bindValue("email", $email);
		//need to delete image
		if(!$stmt->execute())
		{
			throw new \Exception("Fingerprint ".$fingerprint." could not be deleted from database");
		}
		if(!unlink($imageDir.$folder."/".$fingerprint)) {
			throw new \Exception("Fingerprint ".$fingerprint." file could not be deleted");
		}
	} catch(\Exception $e) {
		return $e->getMessage();
	}
}

}
?>
