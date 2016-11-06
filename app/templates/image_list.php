<!DOCTYPE html>
<html>
<head>
	<title>Uploaded Image</title>
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
	</head>
	<body>
		<div id="main" class="container-fluid">
<div class="dropdown">
  <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
    Dropdown
    <span class="caret"></span>
  </button>
  <ul class="dropdown-menu" aria-labelledby="dropdownMenu1">
    <li><a href="#">Action</a></li>
    <li><a href="#">Another action</a></li>
    <li><a href="#">Something else here</a></li>
    <li role="separator" class="divider"></li>
    <li><a href="#">Separated link</a></li>
  </ul>
</div>
			<div id="content" class="clear col-xs-offset-2">
						<h2>Uploaded Images</h2>
						<form method="post" action="processImages">
<?php
if(isset($images)&&!empty($images)) {
	echo  '<div class="panel-group">';
	foreach($images as $image) {
		echo '<div class="panel panel-default row">';
		echo '<div class="panel-heading">';
		echo "<div class='col-md-3'><img src='data:image/".$image["extension"].";base64,".$image["imageFile"]."' style='width:128px;height:128px;'></div>";
		echo "<div class='col-md-5'>";
		if(count($image["found"])) {
			echo "<a class='glyphicon glyphicon-warning-sign' data-toggle='collapse' href='#found".$image["hash"]."'>".$image["orig_name"]."</a>";
		} else {
			echo $image["orig_name"]; 
		}
		echo "</div>";
		echo "<div class='col-md-1'>"."<label><input type='checkbox' value=delete id='".$image["hash"]."' name= '".$image["hash"]."'> Delete</label>"."</div>";
		echo '</div>';
		echo '</div>';
		echo '<div id="found'.$image["hash"].'" class="nav-collapse collapse">';
		if(count($image["found"])==0) {
			echo '<div class="panel-footer">No matches found</div>';
		} else {
			echo '<div class="panel-footer">'.count($image["found"])." match found".'</div>';
		}
		echo '<div class="panel-body">';
		foreach($image["found"] as $found) {
			echo '<div class="row">';
			echo '<div class="col-md-2">'.$found["date"].'</div>';
			echo '<div class="col-md-10">'.$found["address"]."</div>";
			echo '</div>';
		}
		echo '</div>';
		echo '</div>';
	}
	echo '</div>';
}
?>
						<button class="btn btn-lg btn-primary col-md-3" type="submit">Process Images</button>
					</form>
			</div>
		</div>

	</body>
</html>
