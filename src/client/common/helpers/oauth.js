import promiseWindow from 'promise-window';

function popupWindowSize(provider) {
  switch (provider.toLowerCase()) {
    case 'facebook':
      return { width: 580, height: 400 };
    case 'google':
      return { width: 452, height: 633 };
    case 'github':
      return { width: 1020, height: 618 };
    case 'linkedin':
      return { width: 527, height: 582 };
    case 'twitter':
      return { width: 495, height: 645 };
    case 'live':
      return { width: 500, height: 560 };
    case 'yahoo':
      return { width: 559, height: 519 };
    default:
      return { width: 1020, height: 618 };
  }
}

export function authentication(provider, autoLogin = true) {
  const windowSize = popupWindowSize(provider);
  return new Promise((result, reject) => {
    promiseWindow.open('/externalloginredirect?provider=' + provider + '&autoLogin=' + autoLogin, { ...windowSize })
      .then((windowResult) => {
        result(windowResult);
      }, () => {
        reject({});
      });
  });
}