# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|

  config.vm.box = 'ubuntu/trusty32'

  config.vm.define "dev" do |dev|

    dev.vm.hostname = "skimurdev"

    dev.vm.network :private_network, ip: "192.168.10.200"
    dev.vm.network :forwarded_port, guest: 5656, host: 5656, id: "skimurpostgres"
    dev.vm.network :forwarded_port, guest: 5672, host: 5672, id: "skimurrabbitmq"
    dev.vm.network :forwarded_port, guest: 15672, host: 15672, id: "skimurrabbitmqadmin"
    dev.vm.network :forwarded_port, guest: 6379, host: 6379, id: "skimurredis"
    # dev.vm.network :forwarded_port, guest: 9042, host: 9042, id: "skimurcassandra"

    dev.vm.synced_folder "database", "/database"

  end

  config.vm.define "postgres", autostart: false do |postgres|

    postgres.vm.hostname = "skimurpostgres"

    postgres.vm.network :private_network, ip: "192.168.10.201"
    postgres.vm.network :forwarded_port, guest: 5656, host: 5656, id: "skimurpostgres"

    postgres.vm.synced_folder "database", "/database"

  end

  config.vm.define "rabbitmq", autostart: false do |rabbitmq|

    rabbitmq.vm.hostname = "skimurrabbitmq"

    rabbitmq.vm.network :private_network, ip: "192.168.10.202"
    rabbitmq.vm.network :forwarded_port, guest: 5672, host: 5672, id: "skimurrabbitmq"
    rabbitmq.vm.network :forwarded_port, guest: 15672, host: 15672, id: "skimurrabbitmqadmin"

  end

  config.vm.define "redis", autostart: false do |redis|

    redis.vm.hostname = "skimurredis"

    redis.vm.network :private_network, ip: "192.168.10.203"
    redis.vm.network :forwarded_port, guest: 6379, host: 6379, id: "skimurredis"

  end

  config.vm.provision "puppet" do |puppet|
    puppet.module_path = "puppet/modules"
    puppet.manifests_path = "puppet/manifests"
    puppet.manifest_file = "default.pp"
  end

  config.vm.provider "virtualbox" do |v|
    v.memory = 4096
    v.cpus = 2
  end

  config.ssh.forward_agent = true

end
