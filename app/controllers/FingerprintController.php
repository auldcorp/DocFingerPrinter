<?php
namespace Napkins;

require_once(__DIR__ . "/class_template.php");

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;

Class FingerprintController {
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
	$i = 0;
	if ($handle = opendir($imageDir)) {
        	while (($file = readdir($handle)) !== false) {
            		if (!in_array($file, array('.', '..')) && !is_dir($imageDir.$file)) {
                		$i++;
			}
        	}
    	}


		$fileName = "".$i;
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

		imagepng($fingerprint, $imageDir);
		return($fingerprint);
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
}
?>
