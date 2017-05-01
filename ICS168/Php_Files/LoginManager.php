<?php
  $servername = "localhost";
  $serverusername = "root";
  $serverpassword = "";
  $dbName = "teamnewport";

  //Make connection
  $conn = new mysqli($servername, $serverusername, $serverpassword, $dbName);

  $clientUsername = $_POST["usernamePost"];
  $clientPassword = $_POST["passwordPost"];


  //Get user's password
  $sql = "SELECT password FROM users WHERE username = '".$clientUsername."'";
  $result = mysqli_query($conn, $sql);   //Send sql query to server

  //Get result and confirm login
  if(mysqli_num_rows($result) > 0){
    while($row = mysqli_fetch_assoc($result)){
      if($row['password'] == $clientPassword){
        echo "valid";
      }else{
        echo "invalid";
      }
    }
  }else{
    echo "user not found";
  }
 ?>
