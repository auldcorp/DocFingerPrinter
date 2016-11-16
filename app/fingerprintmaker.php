<?php
int $width = 300;
int $height = 300;
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
?>
