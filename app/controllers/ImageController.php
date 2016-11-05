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
	$templating = new WebTemplate();

	$userImages = $app["db"]->fetchAll($sql, Array("email" => $email));
	for($i = 0; $i < count($userImages); ++$i) {
		$imageFile = $imageDir."/".$userImages[$i]["hash"].$thumbnailExtension.".".$userImages[$i]["extension"];
		$userImages[$i]["imageFile"] = base64_encode(file_get_contents($imageFile));
		if($userImages[$i]["imageFile"] == False) {
			array_push($this->errors, "Image file ".$userImages[$i]["orig_name"]." was not found");
		}
		$userImages[$i]["found"] = $app["db"]->fetchAll($foundSQL,["hash" => $userImages[$i]["hash"]]);
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

	$sql = "SELECT * FROM images WHERE hash = CAST(:hash AS UNSIGNED) AND email = :email";
	$image = $app["db"]->fetchAll($sql, ["email" => $email, "hash" => $hash]);
	try {
		if(count($image) != 1) {
			throw new \Exception("Image not found");
		}
		$sqlDeleteImage = "DELETE FROM images WHERE hash = CAST(:hash AS UNSIGNED) AND email = :email";

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
}
?>
