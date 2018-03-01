# Use Heroku Private Space Peering to access Microsoft SQL Server RDS instance from Heroku app

## Introduction

This guide covers how customers can use [Heroku Private Space Peering](https://devcenter.heroku.com/articles/private-space-peering) to access a Microsoft SQL Server database running in a customer VPC from an app deployed in a [Heroku Private Space](https://devcenter.heroku.com/articles/private-spaces). This is useful for customers that want to run apps on Heroku that need to access to legacy database systems running in an existing AWS VPC or for building Heroku apps that require access to database backend's not provided by Heroku or any of Heroku's add-on partners.

By using Private Space Peering, Heroku apps can securely access AWS resources (such as databases) running in non-Heroku VPCs without exposing those resources on the public internet.

## Sample App

This guide uses a [simple ASP.NET Core app](https://github.com/friism/dotnetcore-db-sample) to demonstrate connectivity between apps in a Heroku Private Space and a database running in a customer-managed VPC. The sample app can be backed by either PostgreSQL or Microsoft SQL Server.

The app can be built and deployed on Heroku using either [Docker and the Heroku container registry](https://devcenter.heroku.com/articles/container-registry-and-runtime) or by using the traditional buildpack system with a [3rd party buildpack](https://github.com/friism/dotnetcore-buildpack). Note that .NET Core apps are not formally supported by Heroku. 

## Set up VPC and RDS MS SQL Server Database

This section covers setting up a VPC with an AWS RDS MS SQL Server. You can skip this section if you already have a VPC with a SQL Server instance that you want to consume from an app running in a Heroku private space.

1. Launch the [AWS RDS instance wizard](https://console.aws.amazon.com/rds/home#launch-dbinstance:ct=dbinstances)
1. Make sure you're in the same AWS region as the Heroku Private Space that will be using the database
1. Select Microsoft SQL Server. Express Edition is fine for testing
1. Complete the wizard. You don't have to make the RDS database publicly available

![](/i/rds-network-security.png "RDS database can be private")

## Peer VPC and Heroku Private Space

If you don't already have a Heroku Private Space create one. [Heroku Enterprise](https://www.heroku.com/enterprise#contact) is required to create Private Spaces.

Follow the [Heroku Private Space Peering guide](https://devcenter.heroku.com/articles/private-space-peering). Don't forget to set up routes. Finally, configure your VPC security group to allow ingress traffic from your Heroku dynos to the SQL Server instance.

![](/i/modify-security-group.png "Security group configuration")

## Deploy the app

Clone the sample code:

```
git clone https://github.com/friism/dotnetcore-db-sample
```

Create an app in your private space using the right buildpack. The push the sample app source code:

```
cd dotnetcore-db-sample
heroku apps:create --space <your-space> --buildpack https://github.com/friism/dotnetcore-buildpack
git push heroku master
```

You also need to add a config var with the connection string for your Microsoft SQL Server. You can find the endpoint hostname in the RDS settings.

![](/i/aws-rds-endpoint.png "AWS RDS endpoint hostname")

This example uses the RDS master DB instance user to connect (you provided the password for that user when creating the RDS instance) but you can use other users too:

```
heroku config:set CONNECTION_STRING="Server=friism-test.cmq6ja8aki1m.eu-west-1.rds.amazonaws.com;Database=todo;User=friism;Password=<your-password>;MultipleActiveResultSets=true"
```

You can now open the deployed app:

```
heroku open
```
