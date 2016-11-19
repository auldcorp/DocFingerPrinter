<!DOCTYPE html>
<html>
<head>
	<title>Uploaded Image</title>
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
	<script>
	$(document).ready(function () {
		$("input").change(function () { 
			if(this.checked){ 
				if($("button").hasClass("disabled")) $("button").toggleClass("disabled"); 
			}
			var boxes = $("input");
			var emptyBoxes = [].filter.call(boxes, function(el) {
				return !el.checked;
			});

			if(boxes.length == emptyBoxes.length) {
				if(!$("button").hasClass("disabled")) $("button").toggleClass("disabled");
			}
	});
	});
	</script>
	</head>
	<body>
		<div id="main" class="container-fluid">
			<div id="content" class="clear col-xs-offset-2">
						<h2>Uploaded Images</h2>
						<form method="post" action="processImages">
<?php
if(isset($images)&&!empty($images)) {
	echo  '<div class="panel-group">';
	foreach($images as $image) {
		echo '<div class="panel panel-default row">';
		echo '<div class="panel-heading"></div>';
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
            $grades = array('A', 'B', 'C', 'D');
			echo '<div class="row">';
			echo '<div class="col-md-2">'.$found["date"].'</div>';
			echo '<div class="col-md-4">'."<a href='".$found["address"]."'>".$found["address"]."</a></div>";
			echo '<div class="col-md-2">'."Grade: ".$grades[$found["grade"]]."</div>";
            echo "<button class='btn btn-sm col-md-2' type='submit'>"."Notify"."</button>";
			echo '</div>';
		}
		echo '</div>';
	}
	echo '</div>';
}
?>
						<button class="disabled btn btn-lg btn-primary col-md-3" type="submit">Delete Images</button>
					</form>
			</div>
		</div>

	</body>
</html>
