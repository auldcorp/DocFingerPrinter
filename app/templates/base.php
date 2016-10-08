 <!DOCTYPE html>
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<title>Image Hash</title>
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
	<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">
	<link rel="stylesheet" type="text/css" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css" />



	</head>
	<body>
<div id="bg_page">
	<div id="header">
		<nav class="navbar navbar-default">
			<div class="container-fluid">
				<div class="navbar-header">
					<a class="navbar-brand" href="/">Image Hash</a>
				</div>
				<div class="collapse navbar-collapse">
					<ul id="main-menu" class="nav navbar-nav" >
						<li class="active"><a href="/">Image Hash</a></li>
						<li class="dropdown">
							<a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Data <span class="caret"></span></a>
								<ul class="dropdown-menu" role="menu">
									<li><a href="/import">Import</a></li>
								</ul>
						</li>
					</ul>
					<ul class="nav navbar-right">
						<li><a href="/login"><i class="fa fa-user fa-2x valign-baseline"></i> User</a></li>
					</ul>
				</div>
			</div>
		</nav>
	</div>
	<div id="content" class="clear row">
		
		<?php
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
	<div id="footer" class="clear">
		<p>Capstone</p>
	</div>
</div>
	</body>
</html>
