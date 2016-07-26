import * as React from "react";
import * as ReactDOM from "react-dom";
import {Route,Router, IndexRoute, browserHistory, Link,IndexRedirect} from "react-router";
import {Hello} from "./components/Hello";


export class Frame extends React.Component<any,any>{
  render(){
    return (<div>
      <h1>Netric</h1>
      <ul>
        <li><Link to="/home">home</Link></li>
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

export interface SettingsState
{
  assemblies?:string;
  sites?:string[];
  enabled?:boolean
}

export class Settings extends React.Component<any,SettingsState>{
  constructor(){
    super()
    this.state = {assemblies:"", sites : ["1"],enabled:false}
  }
  componentWillMount(){
      setTimeout(()=>{
        this.setState({assemblies:'1337'})
      },1000)
  }

  onAssembliesChange (event: any){
    this.setState({assemblies: event.target.value})
  }

  handleSubmit (event:any){
    alert(this.state.assemblies)
  }
  render(){
    return <div> <h1>Settings</h1>
      <input value={this.state.assemblies} onChange={this.onAssembliesChange.bind(this)} />
      <br/>
      <select multiple>
        <option>site1</option>
        <option>site2</option>
        <option>site3</option>
      </select>
      <br/>
      <button onClick={this.handleSubmit.bind(this)}>Save</button>
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
