import api from 'helpers/api';

export function getSecurity(store)  {
  return () => {
    return api.getSecurity();
  }
};

export function setTwoFactor(store)  {
  return (enabled) => {
    return api.setTwoFactor(enabled);
  }
};

export function getEmail(store) {
  return () => {
    return api.getEmail();
  };
}

export function changeEmail(store) {
  return (email, emailConfirm, currentPassword) => {
    return api.changeEmail(email, emailConfirm, currentPassword);
  };
}

export function verifyEmail(store) {
  return () => {
    return api.verifyEmail();
  };
}

export function changePassword(store)  {
  return (oldPassword, newPassword, newPasswordConfirm) => {
    return api.changePassword(oldPassword, newPassword, newPasswordConfirm);
  }
};

export function getExternalLogins(store) {
  return () => {
    return api.getExternalLogins();
  }
}

export function addExternalLogin(store) {
  return () => {
    return api.addExternalLogin();
  }
}

export function removeExternalLogin(store) {
  return (loginProvider, providerKey) => {
    return api.removeExternalLogin(loginProvider, providerKey);
  }
}