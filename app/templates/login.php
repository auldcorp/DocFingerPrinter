<!DOCTYPE html>
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
			<title>Login</title>
	</head>
	<body>
	<div class="well col-md-4 col-sm-offset-4">
		<?php $form->start('login', 'post', 'class="form-signin" role="form"');
			if(isset($_REQUEST['return']))
			{
				$form->field('hidden', 'return', NULL, ['value' => $_REQUEST['return']]);
			}
		?>
		<h1><center>Login</center></h1>
		<div class="form-group row">
			<label for="Email" class="col-md-2 control-label">Email</label>
			<div class="col-md-8">
			<input type="text" name="email" class="form-control" <?php if(isset($form->values['email'])) echo 'value="' . $form->values['email'] . '"'; ?>  placeholder="Email">
			</div>
		</div>
		<div class="form-group row">
			<label for="Password" class="col-md-2 control-label">Password</label>
			<div class="col-md-8">
				<input type="password" name="password" class="form-control" placeholder="Password">
			</div>
		</div>
		<a class="co-sm-offset-9" href="register">Register</a>
		<div class="form-group">
			<button class="btn btn-lg btn-primary col-md-8 col-md-offset-2" type="submit">Login</button>
		</div>
		<?php $form->end();?>
	</div>
	</body>
</html>
