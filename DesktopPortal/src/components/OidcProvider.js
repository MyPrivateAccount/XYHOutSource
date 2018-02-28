import React from 'react'
import { OidcProvider } from 'redux-oidc';

class OidcProviderEx extends OidcProvider{
    onUserUnloaded = () => {
       console.log('userUnloaded');
      };
}

export default OidcProviderEx;