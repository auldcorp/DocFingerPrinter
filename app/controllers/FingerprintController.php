<?php
namespace Napkins;

require_once(__DIR__ . '/class_template.php');

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;

Class FingerprintController {
	//creates a png image with ellipses on it
	//TODO this should do other shapes. only ellipses for now for simplicity
	function newFingerprint(){
		$width = 300;
		$height = 300;
		$numShapes = 7;
		header('Content-type: image/png');
		$fingerprint = imagecreatetruecolor($width, $height);//returns false on error or image identifier on success

		if (!$fingerprint) {
print("ERROR");
			return(false);
		} else {

		//loop over  with different shapes and coordinates

			$xCoord = rand(1, $width);
			$yCoord = rand(1, $height);

			$shapeWidth = rand(1,100);
			$shapeHeight = rand(1,100);
			$color = imagecolorallocate($fingerprint, rand(0,255), rand(0,255), rand(0,255)); //random color

			imagefilledellipse($fingerprint, $xCoord, $yCoord, $shapeWidth, $shapeHeight, $color);


		
		imagepng($fingerprint);
		}

		
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
				$img = false;
				break;
		}

		$fingerprint = @imagecreatefrompng($fingerP);

		imagecopymerge($img, $fingerprint, 0, 0, 0, 0, $width, $height); 
		header('Content-type: image/png');
		imagepng($img);

	}
}
?>
