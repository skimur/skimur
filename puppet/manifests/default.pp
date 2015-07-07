node skimurpostgres {

  class { 'skimur::postgres': }

}

node skimurrabbitmq {

  class { 'skimur::rabbitmq': }

}

node skimurredis {

  class { 'skimur::redis': }

}

node skimurdev {

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

  exec { 'create database':
    command     => "${postgresql::server::psql_path} -d skimur -f /database/createdatabase.sql",
    user        => 'postgres',
    logoutput   => true,
    path        => '/usr/bin:/usr/local/bin:/bin',
    require     => ::postgresql::server::db['skimur']
  }

  ::postgresql::server::table_grant { 'users permission':
    privilege => 'ALL',
    table     => 'users',
    db        => 'skimur',
    role      => 'db-user',
    require     => Exec['create database'],
  }

  ::postgresql::server::table_grant { 'roles permission':
    privilege => 'ALL',
    table     => 'roles',
    db        => 'skimur',
    role      => 'db-user',
    require     => Exec['create database'],
  }

  ::postgresql::server::table_grant { 'user roles permission':
    privilege => 'ALL',
    table     => 'user_roles',
    db        => 'skimur',
    role      => 'db-user',
    require     => Exec['create database'],
  }

  ::postgresql::server::table_grant { 'user logins permission':
    privilege => 'ALL',
    table     => 'user_logins',
    db        => 'skimur',
    role      => 'db-user',
    require     => Exec['create database'],
  }

  ::postgresql::server::table_grant { 'subs permission':
    privilege => 'ALL',
    table     => 'subs',
    db        => 'skimur',
    role      => 'db-user',
    require     => Exec['create database'],
  }

  ::postgresql::server::table_grant { 'sub admins permission':
    privilege => 'ALL',
    table     => 'sub_admins',
    db        => 'skimur',
    role      => 'db-user',
    require     => Exec['create database'],
  }

  ::postgresql::server::table_grant { 'sub scriptions permission':
    privilege => 'ALL',
    table     => 'sub_scriptions',
    db        => 'skimur',
    role      => 'db-user',
    require     => Exec['create database'],
  }

  ::postgresql::server::table_grant { 'posts permission':
    privilege => 'ALL',
    table     => 'posts',
    db        => 'skimur',
    role      => 'db-user',
    require     => Exec['create database'],
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
