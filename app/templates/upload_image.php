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
					<form enctype="multipart/form-data" action="import" method="post">
						<h2>Upload Image</h2>
						<label class="btn btn-default btn-file">
								Browse <input type="file" name="userfile" style="display: none;">
						</label>
						<input type="submit" class="btn btn-info" value="Upload">
					</form>	
				</div>
			</div>
		</div>
	</body>
</html>
