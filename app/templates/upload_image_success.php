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
					<h2>Successful Files</h2>
					<ul>
						<?php
foreach($succeeded as $key=>$value) {
		echo "<li>"+$value+"</li>";
}
						?>
					</ul>
					<h2>Failed Files</h2>
					<ul>
						<?php
foreach($failed as $fail) {
		echo "<li>"+$fail+"</li>";
}
						?>
					</ul>
				</div>
			</div>
		</div>
	</body>
</html>
