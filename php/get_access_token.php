<?php

$code = $_POST['code'];

$token = (new PaypingDriver)->getAccessToken($code);
