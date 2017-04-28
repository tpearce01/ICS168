<?php
  $servername = "localhost";
  $serverusername = "root";
  $serverpassword = "";
  $dbName = "teamnewport";

  //Make connection
  $conn = new mysqli($servername, $serverusername, $serverpassword, $dbName);

  //Check connection, in php . is used for concatenation
  // if(!$conn){
  //   die("Connection Failed. ".mysqli_connect_error());
  // } else {
  //   echo ("Connection Success."."</br>");
  // }

  $clientUsername = $_POST["usernamePost"];
  $clientPassword = $_POST["passwordPost"];

  $sql = "SELECT username FROM users WHERE username = '".$clientUsername."'";
  $result = mysqli_query($conn, $sql);

  if(mysqli_num_rows($result) > 0){
    while($row = mysqli_fetch_assoc($result)){
      if($row['username'] == $clientUsername){
        echo "username exists";
      }
    }
  }else{
    $sql = "INSERT INTO users(username, password) VALUES ('".$clientUsername."','".$clientPassword."')";
    $result = mysqli_query($conn, $sql);
    echo "account created";
  }
  // if($result){
  //   echo "username exists";
  // }else{
  //   //Inserting new accounts into database
  //   $sql = "INSERT INTO users(username, password) VALUES ('".$clientUsername."','".$clientPassword."')";
  //   $result = mysqli_query($conn, $sql);
  //   echo "account created";
  // }

  // if(!result) echo "ERROR!";
  // else echo "SUCCESS!";
 ?>
