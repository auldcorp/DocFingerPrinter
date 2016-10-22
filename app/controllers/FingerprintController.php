<?php
int $width = 300;
int $height = 300;
//creates a png image with ellipses on it
//TODO this should do other shapes. only ellipses for now for simplicity
function newFingerprint(){

	int $numShapes = 7;

	$image = imagecreatetruecolor(width, height); //returns false on error or returns image identifier on success

	//loop over  with different shapes and coordinates
	for (int i=0; i<$numShapes; i++){
		int $xCoord = rand(0, width);
		int $yCoord = rand(0, height);

		int $shapeWidth = rand(1,10);
		int $shapeHeight = rand(1,10);
		$color = imagecolorallocate($image, rand(0,255), rand(0,255), rand(0,255)); //ints between 0-255 for red, blue, green

		imagefilledellipse($image, $xCoord, $yCoord, $shapeWidth, $shapeHeight, $color);

		header(‘Content-Type: image/png’);
	}
	imagePNG($image);
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
?>
