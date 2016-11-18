<!DOCTYPE html>
<html>
<head>
	<title>Fingerprints</title>
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
	</head>
	<body>
		<div id="main" class="container-fluid">
			<div id="content" class="clear col-xs-offset-2">
						<h2>Fingerprints</h2>
						<div class="row">
						<a href="fingerprint" class="btn btn-lg btn-primary col-md-3">Create</a>
						</div>
						<form method="post" action="fingerprintAction">
<?php
if(isset($images)&&!empty($images)) {
	echo  '<div class="panel-group">';
	foreach($images as $image) {
		echo '<div class="panel panel-default row">';
		echo '<div class="panel-heading">';
		echo "<div class='col-md-3'><img src='data:image/png;base64,".$image["imageFile"]."' style='width:128px;height:128px;'></div>";
		echo "<div class='col-md-5'>";
		echo $image["fingerprint"]; 
		echo "</div>";
		echo "<div class='col-md-1'>"."<label><input type='checkbox' value=delete id='".$image["fingerprint"]."' name= '".$image["fingerprint"]."'> Delete</label>"."</div>";
		echo '</div>';
		echo '</div>';
	}
	echo '</div>';
}
?>
						<button class="btn btn-lg btn-primary col-md-3" type="submit">Process Fingerprints</button>
					</form>
			</div>
		</div>

	</body>
</html>
