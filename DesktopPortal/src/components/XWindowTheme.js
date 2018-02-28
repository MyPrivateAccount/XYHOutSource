import React from "react"
import { defaultTheme } from './XWindow'
import { TitleBar as RDTitleBar } from 'react-desktop'

const titleHeight = 30;
const fontSize = '1.2rem';
const fontFamily = 'Helvetica, sans-serif';

export let WindowsTheme = {
	xtheme: {
		title: {
			...defaultTheme.title,
			fontFamily: fontFamily,
			color: 'rgba(0, 0, 0, 0.7)',
			fontSize: fontSize,
			height: titleHeight,
			borderBottom: "solid 1px rgba(81, 81, 81, 0.13)"
		},
		frame: {
			...defaultTheme.frame,
			borderWidth: 0,
			boxShadow: '0 2px 11px 3px rgba(0, 0, 0, .2)',
			backgroundColor: '#ffffff',
			boxSizing: 'border-box',
			borderColor: 'transparent',

		},
		transition: 'all 0.25s ease-in-out'
	},
	titleBar: <RDTitleBar controls />
};