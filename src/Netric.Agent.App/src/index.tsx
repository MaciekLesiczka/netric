import * as React from "react";
import * as ReactDOM from "react-dom";
import {Route,Router, IndexRoute, browserHistory, Link,IndexRedirect} from "react-router";
import {Settings} from "./components/Settings";


export class Frame extends React.Component<any,any>{
  render(){
    return (<div>
      <h1>Netric</h1>
      <ul>
        <li><Link to="/home">Home</Link></li>
        <li><Link to="/settings">Settings</Link></li>
      </ul>
      <div>
        {this.props.children}
      </div>
    </div>)
  }
}

export class Home extends React.Component<any,any>{
  render(){
    return <div><h1>Home</h1>

    </div>;
  }
}



let routeMap = (
  <Route  path="/" component={Frame}>
    <IndexRedirect to="/settings" />
    <Route path="/home" component={Home}/>
    <Route path="/settings" component={Settings}/>
  </Route>
)

ReactDOM.render(<Router history={browserHistory}>{routeMap}</Router>, document.getElementById('example'))
