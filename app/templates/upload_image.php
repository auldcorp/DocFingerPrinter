 <!DOCTYPE html>
<html>
<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<title>Upload Image</title>
        <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css" rel="stylesheet">
        <link href="../vendor/kartik-v/bootstrap-fileinput/css/fileinput.css" media="all" rel="stylesheet" type="text/css" />
        <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
        <script src="../vendor/kartik-v/bootstrap-fileinput/js/fileinput.js" type="text/javascript"></script>
        <script src="../vendor/kartik-v/bootstrap-fileinput/js/fileinput_locale_fr.js" type="text/javascript"></script>
        <script src="../vendor/kartik-v/bootstrap-fileinput/js/fileinput_locale_es.js" type="text/javascript"></script>
        <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js" type="text/javascript"></script>

	</head>
	<body>
		<div id="main" class="container-fluid">
			<div id="content" class="clear row col-xs-offset-2">
				<div class="col-md-9">
					<form id="frmmultiple" enctype="multipart/form-data" method="post" action="import">
						<h2>Upload Images</h2>
					  <label class="control-label">Select Files</label>
						<input type="hidden" name="countdiv" id="countdiv" value="">
            <div class="form-group">
            	<input id="file-1" name="form[files][]" class="file" type="file" multiple=true data-preview-file-type="any">
            </div>
        	</form>
				</div>
				<div class="col-md-9">
						<?php
if(isset($succeeded)&&!empty($succeeded)) {
echo '<h2>Successful Files</h2>';
echo '<ul>';
foreach($succeeded as $suc) {
		echo "<li>".$suc."</li>";
}
echo '</ul>';
}
if(isset($failed)&&!empty($failed)) {
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
	<script>
		$("#file-1").fileinput();
	</script>
</html>

