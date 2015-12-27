# Developing with Skimur

It is really easy. Currently, we only support Windows, but that will change soon. Also, it is recommend to use VS2015.

1. Download and install [VirtualBox](https://www.virtualbox.org/wiki/Downloads).
2. Download and install [Vagrant](https://www.vagrantup.com/)
3. Clone the Skimur repository.

```> git clone https://github.com/skimur/skimur```

4. Initialize the virtual the contains all the required services for Skimur to run.

```> vagrant up```

5. Build the project from command line, just to ensure everything is in proper order (it should be!).

```> build```

Your all set!

To start running the website immediately from command line, run the following...

```
> dnvm use default
> cd src/Skimur.Web
> dnx web
```

Now, visit ```http://localhost:5000/``` to visit your instance of Skimur.