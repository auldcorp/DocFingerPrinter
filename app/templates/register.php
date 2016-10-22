<!DOCTYPE html>
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<title>Register</title>

  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
	</head>
	<body>
<?php $form->start('register', 'post', 'class="form-register" role="form"'); 
if(isset($_REQUEST['return']))
			{
				$form->field('hidden', 'return', NULL, ['value' => $_REQUEST['return']]);
			}
?>
<div id="bg_page">
	<div id="content" class="clear row col-xs-offset-2">
		<div class="col-md-9">
		<h1>Register</h1>
		<form action="register/submit" method="post" class="form-horizontal" id="login_form" enctype="multipart/form-data">
 			<div class="form-group">
	  			<label for="Email" class="col-md-4 control-label">Email</label>
				<div class="col-md-8">
				<input type="text" name="email" class="form-control" placeholder="you@email.com">
				</div>
	 		</div>
	 		<div class="form-group">
	  			<label for="Password" class="col-md-4 control-label">Password</label>
				<div class="col-md-8">
				<input type="password" name="password" class="form-control" placeholder="Password">
				</div>
	 		</div>
			<div class="form-group">
				<label for="Password" class="col-md-4 control-label">Verify Password</label>
				<div class="col-md-8">
				<input type="password" name="verify" class="form-control" placeholder="Password">
				</div>
			</div>
			<div class="form-group">
	  			<label for="Full Name" class="col-md-4 control-label">Full Name</label>
				<div class="col-md-8">
				<input type="text" name="full_name" class="form-control" placeholder="Full Name">
				</div>
	 		</div>
	 	<div class="form-group">
			<button class="btn btn-lg btn-primary col-md-8 col-md-offset-4" type="submit">Register</button>
		</div>
		</form>
		</div>

	</div>
</div>
	</body>
</html>
