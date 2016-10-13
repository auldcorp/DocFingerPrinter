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
						<?php
if(!empty($succeeded)) {
echo '<h2>Successful Files</h2>';
echo '<ul>';
foreach($succeeded as $suc) {
		echo "<li>".$suc."</li>";
}
echo '</ul>';
}
if(!empty($failed)) {
echo '<h2>Failed Files</h2>';
echo '<ul>';
foreach($failed as $fail) {
		echo "<li>".$fail."</li>";
}
echo '</ul>';
}
						?>
				</div>
			</div>
		</div>
	</body>
</html>
