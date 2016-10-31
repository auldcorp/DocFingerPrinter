<!DOCTYPE html>
<html>
<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<title>Uploaded Image</title>
	</head>
	<body>
		<div id="main" class="container-fluid">
			<div id="content" class="clear row col-xs-offset-2">
				<div class="col-md-9">
						<h2>Upload Images</h2>
					<?php
if(isset($images)&&!empty($images)) {
echo '<table style="width:100%">';
echo '<tr>';
echo    '<th>Image</th>';
echo    '<th>Name</th>';
echo  '</tr>';
foreach($images as $image) {
	echo "<tr>";
	echo "<th>";
	echo "<img src='data:image/".$image["extension"].";base64,".$image["imageFile"]."' style='width:128px;height:128px;'>";
	echo "</th>";
	echo "<th>".$image["orig_name"]."</th>";
	echo "</tr>";
}
echo '</table>';
}
						?>
				</div>
			</div>
		</div>
	</body>
</html>

