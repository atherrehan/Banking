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

######################                        You will be able to see the like below             ######################

[+] Building 19.1s (18/18) FINISHED                                                                docker:desktop-linux
 => [internal] load build definition from Dockerfile                                                               0.0s
 => => transferring dockerfile: 1.43kB                                                                             0.0s
 => [internal] load metadata for mcr.microsoft.com/dotnet/sdk:9.0                                                  0.0s
 => [internal] load metadata for mcr.microsoft.com/dotnet/aspnet:9.0                                               0.3s
 => [internal] load .dockerignore                                                                                  0.0s
 => => transferring context: 2B                                                                                    0.0s
 => [build 1/9] FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:bb42ae2c058609d1746baf24fe6864ecab0686711dfca1f4b7a9  0.1s
 => => resolve mcr.microsoft.com/dotnet/sdk:9.0@sha256:bb42ae2c058609d1746baf24fe6864ecab0686711dfca1f4b7a99e367a  0.0s
 => [runtime 1/3] FROM mcr.microsoft.com/dotnet/aspnet:9.0@sha256:1af4114db9ba87542a3f23dbb5cd9072cad7fcc8505f6e9  0.1s
 => => resolve mcr.microsoft.com/dotnet/aspnet:9.0@sha256:1af4114db9ba87542a3f23dbb5cd9072cad7fcc8505f6e9131d1feb  0.0s
 => [internal] load build context                                                                                  0.1s
 => => transferring context: 4.94MB                                                                                0.1s
 => CACHED [build 2/9] WORKDIR /src                                                                                0.0s
 => CACHED [runtime 2/3] WORKDIR /app                                                                              0.0s
 => [build 3/9] COPY FinTech.sln ./                                                                                0.1s
 => [build 4/9] COPY Banking.FanFinancing.APIs/Banking.FanFinancing.APIs.csproj Banking.FanFinancing.APIs/         0.1s
 => [build 5/9] COPY Banking.FanFinancing.Domain/Banking.FanFinancing.Domain.csproj Banking.FanFinancing.Domain/   0.1s
 => [build 6/9] COPY Banking.FanFinancing.Infrastructure/Banking.FanFinancing.Infrastructure.csproj Banking.FanFi  0.1s
 => [build 7/9] RUN dotnet restore Banking.FanFinancing.APIs/Banking.FanFinancing.APIs.csproj                     12.7s
 => [build 8/9] COPY . .                                                                                           0.2s
 => [build 9/9] RUN dotnet publish Banking.FanFinancing.APIs/Banking.FanFinancing.APIs.csproj -c Release -o /app/  4.0s
 => [runtime 3/3] COPY --from=build /app/publish .                                                                 0.1s
 => exporting to image                                                                                             0.9s
 => => exporting layers                                                                                            0.7s
 => => exporting manifest sha256:da217d44c9fd9a9cb2aa17c7be820c8f4b39de165b6219d206b70e2e48fa596b                  0.0s
 => => exporting config sha256:4557efa409bb4ed906d831f70b0e623a36005785c0553fb4e2eb7aa925d6ead9                    0.0s
 => => exporting attestation manifest sha256:132857d4f5b08c563f539cd954433b62264fa600ec93e3fde08279c40e0c88e2      0.0s
 => => exporting manifest list sha256:baacfa30fd2d5e68e0f0c684a216a0d78300d26d99b6ed72eda47b8b649bda80             0.0s
 => => naming to docker.io/library/fanfinancing-api:latest                                                         0.0s
 => => unpacking to docker.io/library/fanfinancing-api:latest                                                      0.1s

 ######################                        End             ######################

=> Now, run the Image in container using below command
docker run -d -p 5501:5501 --name fanfinancing-container fanfinancing-api

######################                        You will be able to see the like below             ######################
00f5a48beea2abe4edc5dd2ff09d531ca90a631fc0795504725d338bb88775c7
 ######################                        End             ######################

=> Now, you can access the API on browser/postman etc. using below URL
http://localhost:5501/swagger/index.html


#==========================
#Step 2
#=========================

=>Create a new file named .dockerignore in the root of your project directory
docker run -d -p 5501:5501 --name fanfinancing-container fanfinancing-api
