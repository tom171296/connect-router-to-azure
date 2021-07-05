# Connect router to azure
Example project for connecting an Apache qpid dispatch router to an azure native service

# Prerequisites
To run the router configuration examples in this repository you need the following:

- Have bash installed on your local machine;
- Have [docker(compose)](https://www.docker.com/) installed on your local machine;
- Have an azure subscription;
- Have access to an azure service bus.

# Getting started
The following steps combined with the configuration I explain in my blog "[_Connect a qpid dispatch router to an azure service bus_](https://blognet.tech/2021/ConnectingQpidDispatchToAzure/)" will create a connection between an qpid dispatch router running on your local machine and an azure service bus!

## Step 1: Creating certificates
__NOTE__: this step can be skipped if you are using your own certificate to secure the connection.

The connection between the router and the azure services bus is via AMQPS. This is a protocol where amqp secured by a layer of SSL/TLS. To make use of this security layer, you need to create a certificate that can be used for the connection to azure.

The first step in creating a certificate for the router is to generate a CA certificate. This certificate is used to sign the certificate that is used by the router. To generate the CA certificate you need to run the `generate-ca-cert.sh` in the certificas folder.

The second step in creating certificates is to generate the certificate used by the router itself. This can be done using the script `generate-router-cert.sh`.

Do not change the name of the generated certificates. These are referenced by the [docker-compose file](https://github.com/tom171296/connect-router-to-azure/blob/main/docker-compose.yml) to map a volume and have the certificate available in the container. 

## Step 2: Follow the blog
To get all the configuration the way you want it, I suggest that you read my [blog](https://blognet.tech/2021/ConnectingQpidDispatchToAzure/).

## Step 3: Start the solution
There are three different examples in this solution. Depending on your use case, remove the comments in the docker-compose file to which solution you want to use. 

Run the command `docker-compose up --build` to get the solution working.

Go to [http://localhost:8080] to see your solution and data flowing!