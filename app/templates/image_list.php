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
						<h2>Uploaded Images</h2>
						<form method="post" action="processImages">
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
	echo "<th>"."<label><input type='checkbox' value=delete id='".$image["hash"]."' name= '".$image["hash"]."'> Delete</label>"."</th>";
	echo "</tr>";
}
echo '</table>';
}
						?>
						<div class="col-md-9" align="right">
							<button class="btn btn-lg btn-primary col-md-8 col-md-offset-2" type="submit">Process Images</button>
						</div>
					</form>
				</div>
			</div>
		</div>
	</body>
</html>

