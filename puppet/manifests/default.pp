stage { 'preinstall':
  before => Stage['main']
}

class apt_get_update {
  exec { '/usr/bin/apt-get -y update': }
}

class { 'apt_get_update':
  stage => preinstall
}

node skimurpostgres {

  class { 'skimur::postgres': }

}

node skimurrabbitmq {

  class { 'skimur::rabbitmq': }

}

node skimurredis {

  class { 'skimur::redis': }

}

node skimurcassandra {

  class { 'skimur::cassandra': }

}

node skimurdev {

  class { 'skimur::cassandra': }
  class { 'skimur::postgres': }
  class { 'skimur::rabbitmq': }
  class { 'skimur::redis': }

}

class skimur::postgres {

  $postgres_password = 'password'

  class { 'postgresql::globals':
    manage_package_repo => true,
    encoding            => 'UTF8',
    locale              => 'en_US.utf8'
  } ->
  class { 'postgresql::server':
    listen_addresses           => '*',
    ip_mask_deny_postgres_user => '0.0.0.0/32',
    ip_mask_allow_all_users    => '0.0.0.0/0',
    postgres_password          => $postgres_password,
    port                       => 5656,
  }

  class { 'postgresql::client':
    package_ensure => 'present',
  }
  class { 'postgresql::server::contrib':
    package_ensure => 'present',
  }

  ::postgresql::server::db { 'skimur':
    user     => 'db-user',
    password => $postgres_password,
  }
}

class skimur::rabbitmq {

  include ::apt

  class { '::rabbitmq':
    port              => '5672',
    admin_enable      => true,
    management_port   => '15672',
  }

}

class skimur::redis {

  class { '::redis':
    system_sysctl => true
  }

}

class skimur::cassandra {


  # exec { "apt-update":
  #   command => "/usr/bin/apt-get update"
  # }

  #openjdk-7-jdk
  # class { 'java':
  #   distribution => 'jdk',
  # } ->
  package { [
      'openjdk-7-jdk'
    ]:
    ensure  => 'installed',
  } ->
  class { '::cassandra':
    listen_address  => '192.168.10.200',
    seeds  => '192.168.10.200',
    rpc_address => '192.168.10.200',
    manage_dsc_repo => true
  }

  class { '::cassandra::datastax_agent':
  }

  class { '::cassandra::optutils':
  }
}
