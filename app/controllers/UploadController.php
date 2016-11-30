<?php
namespace Napkins;

require_once(__DIR__ . "/class_template.php");

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Jenssegers\ImageHash\ImageHash;
use Imagick;

Class UploadController {
public function uploadAction(Request $request, Application $app) {
	$imageDir = "";
	if(null == $email = $app["session"]->get("email")) {
		$app["session"]->set("dir", "import");
		return $app->redirect("login");
	}

	$imageDir = $app["imageDirBase"].$email;
	$thumbnailExtension = $app["imageThumbnailExtention"];

	$succeeded = False;
	$data = $request->files->get("form");
	$sql = "INSERT INTO images (hash, email, extension, date, orig_name) VALUES (:hash, :email, :extension, :date, :orig_name)";
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
			$file->move($imageDir, $hash.".".$extension);
			$this->createThumbnail($imageDir."/".$hash.".".$extension, $imageDir."/".$hash.$thumbnailExtension.".".$extension);
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

//TODO function to apply previously created fingerprints to a given image
function mergeFingerprint($file, $fingerP){

        //take $file image and merge with fingerprint $fingerP
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

//TODO fix this. very bad.
//add name to database images (hash, email, image, date)
        $succeed = False;
        $sql = "INSERT INTO images (hash, email, image, date) VALUES (, :email)";
        $stmt = $app["db"]->prepare($sql);
        $stmt->bindValue("i", $num);
        $stmt->bindvalue("email", $email);
        if (!$stmt->execute()){
                throw new \Exception("Failed to merge image with fingerprint");
        }


}

private function createThumbnail($imageFile, $thumbNailFile) {
	$image = new \Imagick(realpath($imageFile));
	$image->setbackgroundcolor('rgb(64,64,64)');
	$image->thumbnailImage(200,200,true);
	$image->writeImage($thumbNailFile);
}
}
?>
