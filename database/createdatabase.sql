CREATE TABLE users
(
  id uuid NOT NULL,
  created_date timestamp with time zone NOT NULL,
  user_name text NOT NULL,
  email text,
  email_confirmed boolean,
  password_hash text,
  security_stamp text,
  phone_number text,
  phone_number_confirmed boolean,
  two_factor_enabled boolean,
  lockout_end_date timestamp with time zone,
  lockout_enabled boolean,
  access_failed_count integer,
  security_question text,
  security_answer text,
  CONSTRAINT users_pkey PRIMARY KEY (id)
);

CREATE TABLE roles
(
  id uuid NOT NULL,
  name text NOT NULL,
  CONSTRAINT roles_pkey PRIMARY KEY (id)
);

CREATE TABLE user_roles
(
  user_id uuid NOT NULL REFERENCES users (id),
  role_id uuid NOT NULL REFERENCES roles (id)
);

CREATE TABLE user_logins
(
  id uuid NOT NULL,
  user_id uuid REFERENCES users (id),
  login_provider text NOT NULL,
  login_key text NOT NULL,
  CONSTRAINT user_logins_pkey PRIMARY KEY (id)
);

CREATE TABLE subs
(
  id uuid NOT NULL,
  created_date timestamp with time zone NOT NULL,
  name text,
  description text,
  sidebar_text text,
  is_default boolean,
  number_of_subscribers bigint,
  CONSTRAINT subs_pkey PRIMARY KEY (id)
);

CREATE TABLE sub_admins
(
  id uuid NOT NULL,
  user_name text,
  sub_name text,
  added_by text,
  added_on timestamp with time zone,
  CONSTRAINT sub_admins_pkey PRIMARY KEY (id)
);

CREATE TABLE sub_scriptions
(
  id uuid NOT NULL,
  user_name text,
  sub_name text,
  CONSTRAINT sub_scriptions_pkey PRIMARY KEY (id)
);

CREATE TABLE posts
(
  id uuid NOT NULL,
  date_created timestamp with time zone NOT NULL,
  last_edit_date timestamp with time zone,
  slug text,
  sub_name text,
  user_name text,
  user_ip text,
  type integer,
  title text,
  content text,
  url text,
  domain text,
  send_replies boolean,
  vote_up_count integer,
  vote_down_count integer,
  CONSTRAINT post_pkey PRIMARY KEY (id)
);

CREATE TABLE votes
(
  id uuid NOT NULL,
  date_created timestamp with time zone,
  user_name text,
  post_slug text,
  type integer,
  date_casted timestamp with time zone,
  ip_address text,
  user_ip text
);