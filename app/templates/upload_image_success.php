 <!DOCTYPE html>
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<title>Upload Image</title>
	</head>
	<body>
		<div id="main" class="container-fluid">
			<div id="content" class="clear row col-xs-offset-2">
				<div class="col-md-9">
					<h4>
						<?php
echo $file;
if($succeeded) {
	echo ' successfuly uploaded';
} else {
	echo ' faild to upload';
}
?>
					</h4>
				</div>
			</div>
		</div>
	</body>
</html>
