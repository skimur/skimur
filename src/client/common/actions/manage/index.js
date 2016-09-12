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