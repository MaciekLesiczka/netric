import React from 'react'
import {render} from 'react-dom'

export default class Settings extends React.Component {

  constructor(){
    super()
    //alert('i am mounted now')
  }
  render(){
    return <div>
              <h1 className="page-header">Settings</h1>
              <form>
                  <fieldset>
                      <div className="form-group">
                          <label htmlFor="asmsInput">{"Assemblies to profile"}</label>
                          <input name="assemblies" type="text" id="asmsInput" value="" className="form-control" placeholder="Company.Product.Web;Company.Product.Feature1*;Company.Product.Feature2*" />
                          <small className="text-muted">
                              {'Provide assembly names, without ".dll" suffix, separated by semicolon. You may use asterisk at the end to provide all assemblies with the same prefix. Changes made in this field requires IIS reset to be applied.'}
                          </small>
                      </div>
                      <div className="form-group">
                          <label htmlFor="sitesSelect">IIS Sites</label>
                          <select name="sites" id="sitesSelect" className="form-control" multiple>
                              <option>tbd</option>
                          </select>
                          <small className="text-muted">
                              Select one or more sites to instrument. Changes here will cause application pool recycle automatically.
                          </small>
                      </div>
                      <button type="submit" className="btn btn-primary">Save</button>
                  </fieldset>
              </form>
            </div>
  }
}
