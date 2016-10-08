<?php

namespace Napkins;

class forms
{
	/* Variables */
	var $name = '';
	var $start_count = 0;
	var $captcha = FALSE;
	var $captcha_step_done = FALSE;
	/* Steps in the form:
		Without CAPTCHA:	show_form, at_form, ready
		With CAPTCHA:		show_form, at_form, show_captcha, at_captcha, ready
	*/
	var $step = 'show_form';
	var $print_html = TRUE;
	var $mode = 'default';
	var $submitted = FALSE;
	var $form_submitted = FALSE;
	var $has_errors = FALSE;
	var $errors = array();
	var $error_ids = array();
	var $values = array();
	
	
	/*
	function to print a form field
		string type, string name, array(parameters)
	*/
	
	function forms($name = 'form', $captcha = FALSE, $print_html = TRUE)
	{
		$this->name = $name;
		$this->captcha = $captcha;
		$this->captcha_step_done = !($captcha);
		$this->print_html = $print_html;
		if(isset($_REQUEST['mode']))
		{
			$this->mode = $_REQUEST['mode'];
		}
		else
		{
			$this->mode = 'default';
		}
		if(isset($_REQUEST['id']))
		{
			$this->id = $_REQUEST['id'];
		} 
		if(isset($_POST['__submit']) && isset($_POST['__form_name']) && $_POST['__form_name'] == $this->name)
		{
			$this->submitted = TRUE;
		}

		/* Find out what step we are at */
		if($this->submitted)
		{
			$this->step = 'at_form';
		}
		if($captcha === TRUE && isset($_POST['g-recaptcha-response']))
		{
			$this->step = 'at_captcha';
		}

		if($this->captcha)
		{
			/* Start a session to pass through form data, if one doesn't already exist */
			$this->check_session();
		}
		if($this->step === 'show_form' || $this->step === 'at_form' || ( !isset($_POST['g-recaptcha-response']) && $this->has_errors ) )
		{
			if(isset($_SESSION)) unset($_SESSION['__post']);
		}
		if($captcha === TRUE && isset($_POST['g-recaptcha-response']) && !isset($_SESSION['__post']))
		{
			// User re-submitted the CAPTCHA page (or has sessions disabled), redirect back to an empty form
			$this->step = 'show_form';
			$this->force_get();
		}
	}
	
	function clear()
	{
		$this->submitted = FALSE;
		$this->has_errors = FALSE;
		$this->errors = array();
		$this->values = array();
		$this->step = 'show_form';
		unset($_POST);
	}
	
	function error($id, $error)
	{
		if(is_array($id))
		{
			$k = current($id);
			$this->errors[$k] = $error;
			foreach($id as $idi)
			{
				$this->error_ids[$idi] = $error;
			}
		}
		else
		{
			$this->errors[$id] = $error;
			$this->error_ids[$id] = $error;
		}
		$this->has_errors = TRUE;
	}
	
	function has_errors()
	{
		return $this->has_errors;
	}
	
	function is_ready()
	{
		/* Backwards compatibility with scripts that don't call post_to_session() */
		if($this->submitted === TRUE && $this->captcha === FALSE && $this->has_errors === FALSE)
		{
			$this->step = 'ready';
		}
		if($this->step === 'ready')
		{
			return TRUE;
		}
		return FALSE;
	}
	
	function is_show_form()
	{
		if($this->step === 'show_form')
		{
			return TRUE;
		}
		return FALSE;
	}
	
	function is_at_form()
	{
		if($this->step === 'at_form')
		{
			return TRUE;
		}
		return FALSE;
	}
		
	function is_at_captcha()
	{
		if($this->step === 'show_captcha' || $this->step === 'at_captcha')
		{
			return TRUE;
		}
		return FALSE;
	}
	
	function check_captcha($recaptcha_private_key, $recaptcha_public_key)
	{
		if(empty($recaptcha_private_key) || empty($recaptcha_public_key))
		{
			throw new Exception('Form class: check_captcha() requires a valid private and public key from reCAPTCHA');
		}
		
		$resp = null;
		if(isset($_POST['g-recaptcha-response']))
		{
			$url = 'https://www.google.com/recaptcha/api/siteverify?secret='
			. urlencode($recaptcha_private_key) . '&response='
			. urlencode($_POST['g-recaptcha-response']) . '&remoteip='
			. $_SERVER['REMOTE_ADDR'];
			$curlinst = curl_init($url);
			curl_setopt($curlinst, CURLOPT_USERAGENT, 'class_forms by WRPN Internet Services');
			curl_setopt($curlinst, CURLOPT_TIMEOUT, 15);
			curl_setopt($curlinst, CURLOPT_FAILONERROR, TRUE);
			curl_setopt($curlinst, CURLOPT_RETURNTRANSFER, TRUE);
			$api_resp = curl_exec($curlinst);
			$resp = json_decode($api_resp);
		}
		
		if(is_object($resp) && isset($resp->success) && $resp->success === TRUE)
		{
			if($this->captcha)
			{
				$_POST = $_SESSION['__post'];
				unset($_SESSION['__post']);
				$this->captcha_step_done = TRUE;
			}
			$this->step = 'ready';
		}
		else
		{
			$this->error('g-recaptcha-response', 'Please complete the "I\'m not a robot" security question.');
			$this->step = 'at_captcha';
		}

	}
	
	function do_captcha($recaptcha_public_key)
	{
		if(empty($recaptcha_public_key))
		{
			throw new Exception('Form class: do_captcha() requires a valid public key from reCAPTCHA');
		}
		$retval = $this->start('', 'post');
		$html = '<p>Please answer the security question below. This helps us verify that you are a human and blocks spam from automated programs.</p>';
		
		$html .= '<div class="g-recaptcha" data-sitekey="' . $recaptcha_public_key . '"></div>';

		$html .= '<p><input type="submit" name="submit" value="Send &raquo;" class="button" /></p>';
		if($this->print_html === TRUE)
		{
			echo $html;
		}
		else
		{
			$retval .= $html;
		}
		$retval .= $this->end();
		if($this->print_html === FALSE)
		{
			return $retval;
		}
	}

	function post_to_session()
	{
		$this->validation_done();
	}
	
	function validation_done()
	{
		if($this->has_errors === FALSE)
		{
			if($this->captcha)
			{
				$_SESSION['__post'] = $_POST;
				$this->step = 'show_captcha';
			}
			else
			{
				$this->step = 'ready';
			}
		}
		else
		{
			$this->step = 'at_form';
		}
	}
	
	function force_get($url = '', $query = '')
	{
		if(empty($url))
		{
			$url = $_SERVER['REQUEST_URI'];
		}
		if($this->mode === 'create')
		{
			$url .= '?created=1';
		}
		else if($this->mode === 'update')
		{
			$url .= '?updated=1';
		}
		else if($this->mode === 'delete' || $this->mode === 'delete_confirm')
		{
			$url .= '?deleted=1';
		}
		header('Location: ' . $url, TRUE, 303);
	}
	
	function gpc($name)
	{
		global $db;
		if(isset($_REQUEST[$name]))
		{
			$temp = $db->prep_gpc($_REQUEST[$name]);
		}
		else if(isset($_POST[$name]))
		{
			$temp = $db->prep_gpc($_POST[$name]);
		}
		else
		{
			$temp = '';
		}
		return $temp;
	}
	
	function show_result($success = '')
	{
		if($this->has_errors === TRUE)
		{
			return $this->show_errors();
		}
		else if($this->submitted === TRUE && $this->captcha === FALSE)
		{
			$this->step = 'ready';
		}
		
		if($this->is_ready())
		{
			return $this->show_success($success);
		}
	}
	
	function show_errors()
	{
		if($this->has_errors === TRUE)
		{ 
			if($this->print_html === TRUE)
			{
				echo '<div class="error"><p><b>The following errors have occurred:</b></p><ul>' . "\n";
				foreach($this->errors as $error_msg)
				{
					echo '<li>' . $error_msg . "</li>\n";
				}
				echo '</ul></div>';
			}
			else
			{
				$return = '<div class="error"><p><b>The following errors have occurred:</b></p><ul>' . "\n";
				foreach($this->errors as $error_msg)
				{
					$return .= '<li>' . $error_msg . "</li>\n";
				}
				$return .= '</ul></div>';
				return $return;
			}
		}
		else if($this->submitted === TRUE && $this->captcha === FALSE)
		{
			$this->step = 'ready';
		}
	}
	
	function show_success($msg)
	{

		if(strlen($msg) !== 0)
		{
			if($this->print_html === TRUE)
			{
				echo '<div class="success">' . $msg . '</div>';
			}
			else
			{
				return '<div class="success">' . $msg . '</div>';
			}
		}
	}
	
	function field($type, $name, $id = '', $param = array(), $options = array())
	{
		$style_done = FALSE;
		$class_done = FALSE;
		$value = '';
		$already_checked = FALSE;
		$name_stripped = preg_replace('/\[[0-9]*\]$/', '', $name);
		
		/* Only text, textarea, and select fields can have their values changed by the user. Other field types are 
		either blank ("password") or will use their original set values ("radio", "checkbox") but be selected by the user */
		 /* For HTML pages with multiple forms, use __form_name to make sure we only set variables for the correct form section */
		if($this->form_submitted() && isset($_POST[$name]) && ($type == 'text' || $type == 'textarea' || $type == 'select' || $type == 'email'))
		{
			$value = $_POST[$name];
		}
		/* Use values set in the script before the form is processed (e.g. from a database query)
		
		radio elements: The value of multiple radio buttons could be overwritten by the user or by setting $this->values['name']
		(since they share the same name, allowing only one selection) so we skip this */
		else if(isset($this->values[$name]) && $type != 'radio')
		{
			$value = (string)$this->values[$name];
		}
		/* Use the original set value */
		else if(isset($param['value']))
		{
			$value = (string)$param['value'];
		}

		
		// var_dump($type, $name, $options, $_POST[$name], $value, $param['value'], $this->values[$name]);
		
		$html = '<';
		if($type == 'text' || $type == 'radio' || $type == 'checkbox' || $type == 'hidden' || $type == 'password' || $type == 'button' || $type == 'submit' || $type == 'file')
		{
			$html .= 'input type="' . $type . '"';
		}
		else if($type == 'select')
		{
			$html .= 'select';
		}
		else if($type == 'textarea')
		{
			$html .= 'textarea';
		}
		else
		{
			// We've found an HTML5 name, or invalid field type. Draw a standard text entry field
			$html .= 'input type="' . $type . '"';
			$type = 'text';
		}
		$html .= ' name="'. $name . '"' . (strlen((string)$id) ? ' id="' . (string)$id . '"' : '');
		
		if(strlen($value) && ($type != 'textarea' && $type != 'password' && $type != 'select' && $type != 'file'))
		{
			/* Some form elements don't use a value parameter; others shouldn't have one for security reasons ("password", "file")
			values for "textarea" and "select" elements are set below */
			$html .= ' value="' . htmlspecialchars($value) . '"';
		}
				
		if(	($type == 'radio' && $this->form_submitted() && isset($_POST[$name]) && $_POST[$name] == $value) || 
			($type == 'radio' && ! $this->form_submitted() && isset($param['value']) && isset($this->values[$name]) && $param['value'] == $this->values[$name]) || 
			($type == 'checkbox' && $this->form_submitted() && isset($_POST[$name_stripped]) && is_string($_POST[$name_stripped]) && $_POST[$name_stripped] == $value) || 
			($type == 'checkbox' && $this->form_submitted() && isset($_POST[$name_stripped]) && is_array($_POST[$name_stripped]) && in_array($value, $_POST[$name_stripped])) ||
			(isset($this->checkbox[$name_stripped]) && isset($this->checkbox[$name_stripped]['on']) && ($value == $this->checkbox[$name_stripped]['on'])) 
		)
		{
			$html .= ' checked="checked"';
			$already_checked = TRUE;
		}
		
		/* Configure the style attribute, if any */
		if(isset($param['style']))
		{
			$html .= ' style="' . $param['style'];
			if(substr($html, -1, 1) !== ';')
			{
				$html .= ';';
			}
			$style_done = TRUE;
		}
		if(isset($param['width']))
		{
			if($style_done === TRUE)
			{
				$html .= ' width: ' . $param['width'] . ';';
			}
			else
			{
				$html .= ' style="width: ' . $param['width'] . ';';
				$style_done = TRUE;
			}
		}
		if($style_done === TRUE)
		{
			$html .= '"';
		}
		
		/* Configure the class attribute, if any */
		if(isset($param['class']))
		{
			$html .= ' class="' . htmlspecialchars($param['class']);
			$class_done = TRUE;
		}
		if($this->form_submitted() && isset($this->error_ids[$name_stripped]))
		{
			if($class_done === TRUE)
			{
				$html .= ' field_error';
			}
			else
			{
				$html .= ' class="field_error';
				$class_done = TRUE;
			}
		}
		if($class_done === TRUE)
		{
			$html .= '"';
		}
		
		if(is_array($param))
		{
			foreach($param as $attrib => $data)
			{
				if($attrib == 'checked' && $data === TRUE && ! $this->form_submitted() && $already_checked === FALSE && !isset($this->checkbox[$name]))
				{
					$html .= ' checked="checked"';
				}
				else if($attrib == 'disabled' && $data === TRUE)
				{
					$html .= ' disabled="disabled"';
				}
				else if($attrib != 'style' && $attrib != 'width' && $attrib != 'class' && $attrib != 'checked' && $attrib != 'disabled' && $attrib != 'value')
				{
					$html .= ' ' . $attrib . '="'. htmlspecialchars($data) . '"';
				}
			}
		}
		
		if($type == 'select')
		{
			$html .= ">\n";
			if(is_array($options))
			{	
				foreach($options as $key => $option_value)
				{
					$html .= '<option value="' . htmlspecialchars($key) . '"';
					if($value == $key)
					{
						$html .= ' selected="selected"';
					}
					$html .= '>' . htmlspecialchars($option_value) . "</option>\n";
				}
			}
			$html .= "</select>\n";
		}
		else if($type == 'textarea')
		{
			$html .= '>' . htmlspecialchars($value) . '</textarea>' . "\n";
		}
		else
		{
			if($type == 'radio' || $type == 'checkbox')
			{
				$html .= ' />';
			}
			else
			{
				$html .= ' />' . "\n";
			}
		}
		
		if($this->print_html === TRUE)
		{
			echo $html;
			return '';
		}
		else
		{
			return $html;
		} 
	}
	
	function button($label, $name = '', $id = '', $param = array())
	{
		$param['value'] = $label;
		return $this->field('button', $name, $id, $param);
	}
	
	function submit($label, $name = 'submit_button', $id = '', $param = array())
	{
		$param['value'] = $label;
		return $this->field('submit', $name, $id, $param);
	}
	
	function label($id, $label, $param = array())
	{
		$param_str = '';
		if(is_array($param))
		{
			foreach($param as $attrib => $data)
			{
				$param_str .= ' ' . $attrib . '="'. $data . '"';
			}
		}
		else
		{
			// Compatibility for when param was a string
			$param_str = $param;
		}
		$html = '<label for="' . $id . '"' . (($param_str !== '') ? ' ' . $param_str : '') . '>' . $label . '</label>';
		if($this->print_html === TRUE)
		{
			echo $html;
		}
		else
		{
			return $html;
		}
	}
	
	function start($url = '', $method = 'post', $param = '')
	{
		$this->start_count++;
		if($this->name === 'form' && $this->start_count > 1)
		{
			$form_name = $this->name . $this->start_count;
		}
		else
		{
			$form_name = $this->name;
		}
		$html = '<form action="' . (empty($url) ? $_SERVER['REQUEST_URI'] : $url) . '" method="'. strtolower($method) . '" id="' . $form_name . '"' . (!empty($param) ? ' ' . $param : '') . '>' . "\n" . 
		'<div>' . "\n" . '<input type="hidden" name="__submit" value="1" />' . "\n" . 
		'<input type="hidden" name="__form_name" value="' . $form_name . '" />' . "\n";
		if($this->mode !== 'default')
		{
			$html .= '<input type="hidden" name="mode" value="' . $this->mode . '" />' . "\n";
		}
		if(isset($this->id))
		{
			$html .= '<input type="hidden" name="id" value="' . $this->id . '" />' . "\n";
		}
		if($this->print_html === TRUE)
		{
			echo $html;
		}
		else
		{
			return $html;
		}
	}
	
	function end()
	{
		$html = "</div>\n</form>\n\n";
		if($this->captcha) $html .= '<script src="https://www.google.com/recaptcha/api.js"></script>' . "\n\n";
		if($this->print_html === TRUE)
		{
			echo $html;
		}
		else
		{
			return $html;
		}
	}
	
	function form_submitted()
	{
		if(isset($_POST['__form_name']) && $_POST['__form_name'] == $this->name) return TRUE;
		return FALSE;
	}

	function check_session()
	{
		/* If no session exists, start a temporary one to secure the form and store form data for processing after the CAPTCHA */
		if(!session_id())
		{
			@session_cache_limiter('none');
			header('Cache-Control: private, must-revalidate');
			session_name('session_forms');
			session_start();
		}
		if(!ob_get_level()) ob_start();
	}
	
	/* Some validation functions */
	
	/* Returns TRUE if e-mail address ($email) is valid, FALSE otherwise */
	function check_email($email)
	{
		return (filter_var($email, FILTER_VALIDATE_EMAIL) !== FALSE);
	}
	
	/* Returns TRUE if URL ($url) is valid, FALSE otherwise */
	function check_url($url)
	{
		return (filter_var($url, FILTER_VALIDATE_URL) !== FALSE);
	}
	
	/* Some widely-used arrays */
	
	/* List of US States */
	var $state_list = array(
	'AL'=>'Alabama',
	'AK'=>'Alaska', 
	'AZ'=>'Arizona', 
	'AR'=>'Arkansas', 
	'CA'=>'California', 
	'CO'=>'Colorado', 
	'CT'=>'Connecticut', 
	'DE'=>'Delaware', 
	'DC'=>'District of Columbia', 
	'FL'=>'Florida', 
	'GA'=>'Georgia', 
	'HI'=>'Hawaii', 
	'ID'=>'Idaho', 
	'IL'=>'Illinois', 
	'IN'=>'Indiana', 
	'IA'=>'Iowa', 
	'KS'=>'Kansas', 
	'KY'=>'Kentucky', 
	'LA'=>'Louisiana', 
	'ME'=>'Maine', 
	'MD'=>'Maryland', 
	'MA'=>'Massachusetts', 
	'MI'=>'Michigan', 
	'MN'=>'Minnesota', 
	'MS'=>'Mississippi', 
	'MO'=>'Missouri', 
	'MT'=>'Montana',
	'NE'=>'Nebraska',
	'NV'=>'Nevada',
	'NH'=>'New Hampshire',
	'NJ'=>'New Jersey',
	'NM'=>'New Mexico',
	'NY'=>'New York',
	'NC'=>'North Carolina',
	'ND'=>'North Dakota',
	'OH'=>'Ohio', 
	'OK'=>'Oklahoma', 
	'OR'=>'Oregon', 
	'PA'=>'Pennsylvania', 
	'RI'=>'Rhode Island', 
	'SC'=>'South Carolina', 
	'SD'=>'South Dakota',
	'TN'=>'Tennessee', 
	'TX'=>'Texas', 
	'UT'=>'Utah', 
	'VT'=>'Vermont', 
	'VA'=>'Virginia', 
	'WA'=>'Washington', 
	'WV'=>'West Virginia', 
	'WI'=>'Wisconsin', 
	'WY'=>'Wyoming'
	);
	function get_state_array($default_title = '')
	{
		return array_merge(array('' => $default_title), $this->state_list);
	}

}
