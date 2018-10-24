<?php

$url = (new PaypingDriver)->getAuthLink();

header('Location: ' . $url);
