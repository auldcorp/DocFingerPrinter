 <!DOCTYPE html>
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<title>TrackIt</title>
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
	<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">
	<link rel="stylesheet" type="text/css" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css" />
	<style>
		#footer {
			background-color: #2a2730;
			color: #99979C;
		}
		#content {
			padding-bottom: 70px;
		}
	</style>

	</head>
	<body>
<div id="bg_page">
	<div id="header">
		<nav class="navbar navbar-default">
			<div class="container-fluid">
				<div class="navbar-header">
					<a class="navbar-brand" href="/">TrackIt</a>
				</div>
				<div class="collapse navbar-collapse">
					<ul id="main-menu" class="nav navbar-nav" >
						<li><a href="import">Import</a></li>
						<li><a href="images">Images</a><li>
						<li><a href="fingerprint">Fingerprints</a></li>
					</ul>
					<ul class="nav navbar-right">
						<?php if(isset($login) && $login){?>
						<li><a href="logout"><i class="fa fa-sign-out fa-2x valign-baseline"></i>Logout</a></li>
						<?php }else{?>
						<li><a href="login"><i class="fa fa-user fa-2x valign-baseline"></i>User</a></li>
						<?php }?>
					</ul>
				</div>
			</div>
		</nav>
	</div>
	<div id="content" class="clear row">

		<?php
		if(isset($success))
		{
			foreach($success as $message)
			{
				echo '<div class="alert alert-success alert-dismissible fade in" role="alert">';
					echo '<button type="button" class="close" data-dismiss="alert" aria-label="Close">';
						echo '<span aria-hidden="true">x</span>';
					echo '</button>';
					echo $message;
				echo '</div>';
			}
		}
		if(isset($errors))
		{
			foreach($errors as $error)
			{
				echo '<div class="alert alert-danger alert-dismissible fade in" role="alert">'; 
					echo '<button type="button" class="close" data-dismiss="alert" aria-label="Close">';
						echo '<span aria-hidden="true">×</span>';
					echo '</button>';
					echo $error;
 				echo '</div>';	
			}
		}

		?>
		

			<?php echo $page_content; ?>


	</div>
	<div id="footer" class="clear navbar navbar-fixed-bottom">
		<div class="container">
		<p>Capstone</p>
		</div>
	</div>
</div>
	</body>
</html>
