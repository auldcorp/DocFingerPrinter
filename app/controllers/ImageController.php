<?php
namespace Napkins;

require_once(__DIR__ . "/class_template.php");

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Jenssegers\ImageHash\ImageHash;

Class ImageController {
public function imageView(Request $request, Application $app) {
	if(null == $email = $app["session"]->get("email"))
	{
		$app["session"]->set("dir", "import");
		return $app->redirect("images");
	}
	$imageDir = $app["imageDirBase"].$email;
	$sql = "SELECT * FROM images WHERE email = :email";
	$templating = new WebTemplate();

	$userImages = $app["db"]->fetchAll($sql, Array("email" => $email));
	for($i = 0; $i < count($userImages); ++$i) {
		$imageFile = $imageDir."/".$userImages[$i]["hash"].".".$userImages[$i]["extension"];
		$userImages[$i]["imageFile"] = base64_encode(file_get_contents($imageFile));
//		var_dump($imageFile);
	}
//	var_dump($userImages);
	$return = ["images"=>$userImages];
	$page_content = $templating->render("image_list.php", $return);

	$templating->setTitle("Upload Image");
	$templating->addGlobal("page_content", $page_content);
	$templating->addGlobal("login", TRUE);

	return $templating->renderDefault();

}
}
?>
