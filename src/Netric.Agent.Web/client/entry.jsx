import Settings from './settings.jsx'
import Home from './home.jsx'
import React from 'react'
import {render} from 'react-dom'
import { Router, Route,IndexRedirect, Link, browserHistory } from 'react-router'

class App extends React.Component {
  static propTypes(){
    return {name:React.propTypes.string.isRequired}
  }
  render () {
    return <div className="row">
              <div className="col-sm-3 col-md-2 sidebar">
                  <ul className="nav nav-sidebar">
                      <li ><Link to="/home" activeClassName="active">Home</Link></li>
                      <li><Link to="/settings" activeClassName="active">Settings</Link></li>
                  </ul>
              </div>
              <div className="col-sm-9 col-sm-offset-3 col-md-10 col-md-offset-2 main">
                {this.props.children}
              </div>
          </div>
  }
}

render(
  <Router history={browserHistory}>
      <Route path="/" component={App}>
        <IndexRedirect to="/home" />
        <Route path="/home" component={Home} />
        <Route path="/settings" component={Settings}/>
      </Route>
    </Router>, document.getElementById('app')
  );
