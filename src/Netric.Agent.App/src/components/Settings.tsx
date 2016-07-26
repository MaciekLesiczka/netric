import * as React from "react";
import * as ReactDOM from "react-dom";

interface SettingsState
{
  assemblies?:string
  sites?:Site[];
  isLoading?:boolean
  message?:string
}

interface Site{
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
    this.setState({message:"Saving..."})
    fetch('/api/settings',{
      method:'POST',
      body:JSON.stringify(this.state)
    } )
    .then(_=>this.setState({message:"Settings saved"}))
  }

  render(){

    let message = this.state.isLoading ? 'Loading...' : this.state.message;

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
