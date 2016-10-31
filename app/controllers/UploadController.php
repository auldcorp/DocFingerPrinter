<?php
namespace Napkins;

require_once(__DIR__ . "/class_template.php");

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Jenssegers\ImageHash\ImageHash;

Class UploadController {
public function uploadAction(Request $request, Application $app) {
	$imageDir = "";
	if(null == $email = $app["session"]->get("email")) {
		$app["session"]->set("dir", "import");
		return $app->redirect("login");
	} else {
		$imageDir = $app["imageDirBase"].$email;
	}
	$succeeded = False;
	$data = $request->files->get("form");
	$sql = "INSERT INTO images (hash, email, extension, date, orig_name) VALUES (CAST(:hash AS UNSIGNED), :email, :extension, :date, :orig_name)";
	$succeeded = [];
	$failed = [];
	foreach($data["files"] as $file) {
		try {
			if($file == Null) {
				return $this->uploadView($request, $app);
			}
			if(!preg_match("/^image\/[a-zA-Z|\-]{2,10}/",$file->getMimeType())) {
				throw new \Exception("File not of Image Type (".$file->getMimeType().")");
			}
			$extension = $file->guessExtension();
			$hasher = new ImageHash(Null, ImageHash::DECIMAL);
			$hash = $hasher->hash($file->getPath()."/".$file->getFilename());

			$stmt = $app["db"]->prepare($sql);
			$stmt->bindValue("hash", $hash);
			$stmt->bindValue("email", $email);
			$stmt->bindValue("extension",$extension);
			$stmt->bindValue("date", date("y-m-d H:i")); //See php date
			$stmt->bindValue("orig_name", $file->getClientOriginalName());
			if(!$stmt->execute())
			{
				throw new \Exception("File could not be uploaded");
			} 
			$hashList = $app["db"]->fetchall("SELECT * FROM images WHERE hash = CAST(:hash AS UNSIGNED)", array("hash" => $hash));
			$file->move($imageDir, $hashList[0]["hash"].".".$extension);
			array_push($succeeded, $file->getClientOriginalName());
		} catch(\Doctrine\DBAL\Exception\UniqueConstraintViolationException $e) { 
			array_push($failed, $file->getClientOriginalName()." is already in the database");
		} catch(\Exception $e) {
			array_push($failed, $file->getClientOriginalName()." -- ".$e->getMessage());
		}
	}
	$return = ["succeeded"=>$succeeded,"failed"=>$failed];
	return $this->uploadView($request, $app, $return);
}
public function uploadView(Request $request, Application $app, Array $var_content = []) {
	if(null == $email = $app["session"]->get("email"))
	{
		$app["session"]->set("dir", "import");
		return $app->redirect("login");
	}
	$templating = new WebTemplate();

	$page_content = $templating->render("upload_image.php", $var_content);

	$templating->setTitle("Upload Image");
	$templating->addGlobal("page_content", $page_content);
	$templating->addGlobal("login", TRUE);

	return $templating->renderDefault();
}
}
?>
