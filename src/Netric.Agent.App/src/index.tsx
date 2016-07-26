import * as React from "react";
import * as ReactDOM from "react-dom";
//import * as F from "whatwg-fetch"
import {Route,Router, IndexRoute, browserHistory, Link,IndexRedirect} from "react-router";
import {Hello} from "./components/Hello";


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

export interface SettingsState
{
  assemblies?:string
  sites?:Site[];
  isLoading?:boolean
  message?:string
}

export interface Site{
  name?:string
  installed?:boolean
}

export class Settings extends React.Component<any,SettingsState>{
  constructor(){
    super()
    this.state = {assemblies:"", sites : [],isLoading:true}
  }
  componentWillMount(){
    let that = this;
    fetch('/api/settings').then(x=>{
      x.json().then(x=>
        {
          x.isLoading = false;
          that.setState(x);
        }
      );
    }, error=> {
      alert(error);//TODO: error handling
    })
  }
  onAssembliesChange (event: any){
    this.setState({assemblies: event.target.value})
  }
  onSitesChange(event:any){
    let sites = this.state.sites.map( site=> {
      site.installed = false;
      for(let installed of event.target.selectedOptions){
          if(site.name === installed.value){
            site.installed = true
          }
      }
      return site;
    } )
    this.setState({sites:sites})
  }
  handleSubmit (){
    this.setState({message:"saving..."})
    fetch('/api/settings',{
      method:'POST',
      body:JSON.stringify(this.state)
    } )
    .then(_=>this.setState({message:"Settings saved"}))
  }

  render(){

    let message = this.state.isLoading ? 'loading...' : this.state.message;

    let content:JSX.Element;
    if(!this.state.isLoading){
      let installedSites = this.state.sites.filter(s=> s.installed).map(s=>s.name);
      content = (
        <div>
          <input value={this.state.assemblies} onChange={this.onAssembliesChange.bind(this)} />
          <br/>
          <select multiple={true} value={installedSites} onChange={this.onSitesChange.bind(this)}>
          {
              this.state.sites.map((s)=>{
                return <option key={s.name}>{s.name}</option>
              })
          }
          </select>
          <br/>
          <button onClick={this.handleSubmit.bind(this)}>Save</button>
        </div>
      )
    }
    return <div>
            <h1>Settings</h1>
            <hr/>
            <span>{message}</span>
            <br/>
            {content}
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
