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
		//THIS NEEDS CHANGED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		$imageFile = $imageDir."/".$userImages[$i]["name"].".".$userImages[$i]["extension"];
		$userImages[$i]["imageFile"] = base64_encode(file_get_contents($imageFile));
		if($userImages[$i]["imageFile"] == False) {
			array_push($this->errors, "Finger print file ".$userImages[$i]["orig_name"]." was not found");
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
}
