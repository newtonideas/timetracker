import React, { Component } from 'react'

class Filters extends Component {
    render() {
        return (
            <div>
                <div className='filter'>Today</div>
                <div className='filter'>Weekly</div>
                <div className='filter'>Stats</div>
                <div className='filter'>Filters</div>
            </div>
        )
    }
}

export default Filters