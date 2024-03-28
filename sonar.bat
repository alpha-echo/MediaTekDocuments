SonarScanner.MSBuild.exe begin /k:"MediaTekDocuments" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="sqp_41c07b5af007b1a03488e07b36dc0ffa67129a32"
MsBuild.exe /t:Rebuild
SonarScanner.MSBuild.exe end /d:sonar.token="sqp_41c07b5af007b1a03488e07b36dc0ffa67129a32"