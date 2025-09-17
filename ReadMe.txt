#==========================
#Requirements
#=========================

=>Make sure docker is installed

=>Make sure that updated sdk is installed, if not, run below commands

docker logout mcr.microsoft.com
docker pull mcr.microsoft.com/dotnet/sdk:9.0


#==========================
#Step 1
#=========================

=>Inside Banking.FanFinancing.APIs project, first setup the Dockerfile (Install docker on your machine first, if not installed)

=>Goto solution path, open terminal and run below command
docker build -t fanfinancing-api -f Banking.FanFinancing.APIs/Dockerfile .

=> Now, run the Image in container using below command
docker run -d -p 5501:5501 --name fanfinancing-container fanfinancing-api

=> Now, you can access the API on browser/postman etc. using below URL
http://localhost:5501/swagger/index.html