import React, { Component } from 'react'
import $ from 'jquery';


class Menu extends Component {
    render() {
        const {task} = this.props
        return (
            <div>
                <script>
                    jQuery(document).ready(function( $ ) {
                    $("#menu").mmenu()
                });
                </script>


        <div class="page">
            <div class="header">
            <a href="#menu"></a>
        Demo
        </div>
        <div class="content">
            <p><strong>This is a demo.</strong><br />
                Click the menu icon to open the menu.</p>
        </div>
        </div>


        <nav id="menu">
            <ul>
                <li><a href="#">Home</a></li>
                <li><a href="#">About us</a></li>
                <li><a href="#">Contact</a></li>
            </ul>
        </nav>
            </div>

        )
    }
}

export {Menu};