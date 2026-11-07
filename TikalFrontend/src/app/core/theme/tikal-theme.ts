import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

const TikalTheme = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#eef6f1',
      100: '#e3f1e8',
      200: '#c3e0cd',
      300: '#97c8a9',
      400: '#63a97f',
      500: '#3d8a5f',
      600: '#2a7049',
      700: '#1f5c3f',
      800: '#174a32',
      900: '#123a27',
      950: '#0a2116',
      color: 'light-dark({primary.700}, {primary.400})',
      contrastColor: 'light-dark(#ffffff, {surface.950})',
      hoverColor: 'light-dark({primary.800}, {primary.300})',
      activeColor: 'light-dark({primary.900}, {primary.200})',
    },
    surface: {
      0: '#ffffff',
      50: 'light-dark(#faf9f5, #f1f3ee)',
      100: 'light-dark(#f5f3ed, #dde2da)',
      200: 'light-dark(#ece9e0, #bcc4b9)',
      300: 'light-dark(#e2ded3, #98a395)',
      400: 'light-dark(#d6d1c4, #768273)',
      500: 'light-dark(#8e958c, #586356)',
      600: 'light-dark(#6f776f, #3f4a3f)',
      700: 'light-dark(#5c675f, #2c362d)',
      800: 'light-dark(#3a443d, #1e271f)',
      900: 'light-dark(#1c2620, #151c16)',
      950: 'light-dark(#111813, #0d120e)',
    },
    text: {
      color: 'light-dark({surface.900}, {surface.0})',
      hoverColor: 'light-dark({surface.950}, {surface.0})',
      mutedColor: 'light-dark({surface.700}, {surface.300})',
      hoverMutedColor: 'light-dark({surface.800}, {surface.200})',
    },
    content: {
      borderRadius: '{border.radius.xl}',
      background: 'light-dark({surface.0}, {surface.900})',
      hoverBackground: 'light-dark({surface.50}, {surface.800})',
      borderColor: 'light-dark({surface.300}, {surface.700})',
    },
    formField: {
      background: 'light-dark({surface.0}, {surface.950})',
      borderColor: 'light-dark({surface.400}, {surface.600})',
      hoverBorderColor: 'light-dark({surface.500}, {surface.500})',
      focusBorderColor: '{primary.color}',
      borderRadius: '{border.radius.lg}',
      focusRing: {
        width: '3px',
        style: 'solid',
        color: 'color-mix(in srgb, {primary.color}, transparent 80%)',
        offset: '0',
        shadow: 'none',
      },
    },
    highlight: {
      background: 'light-dark({primary.100}, color-mix(in srgb, {primary.400}, transparent 84%))',
      focusBackground:
        'light-dark({primary.200}, color-mix(in srgb, {primary.400}, transparent 76%))',
      color: 'light-dark({primary.800}, rgba(255,255,255,.87))',
      focusColor: 'light-dark({primary.900}, rgba(255,255,255,.87))',
    },
  },
  components: {
    chip: {
      root: {
        borderRadius: '{border.radius.sm}',
        paddingX: '0.5rem',
        paddingY: '0.25rem',
        background: 'light-dark({surface.200}, {surface.700})',
        color: 'light-dark({surface.800}, {surface.100})',
      },
    },
    message: {
      contrast: {
        background: 'light-dark({primary.700}, {primary.400})',
        borderColor: 'light-dark({primary.700}, {primary.400})',
        color: 'light-dark(#ffffff, {surface.950})',
      },
      secondary: {
        background: 'light-dark({surface.100}, {surface.800})',
        borderColor: 'light-dark({surface.300}, {surface.700})',
        color: 'light-dark({surface.900}, {surface.100})',
      },
    },
  },
});

export default TikalTheme;
