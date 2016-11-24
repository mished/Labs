import 'babel-polyfill'
import createRenderer from '../canvas/renderer'
import createTool from '../tools/tool'
import { fromRGBA } from '../utils/color'
import './style.css'

export default function init (app) {
  const appContainer = document.createElement('div')
  appContainer.setAttribute('class', 'mg')
  const canvas = document.createElement('canvas')
  canvas.setAttribute('class', 'mg-canvas')
  canvas.width = app.clientWidth
  canvas.height = app.clientHeight - 150

  const renderer = createRenderer(canvas)
  let currentTool

  const tools = [
    {
      title: 'Line',
      tool: createTool({ renderer, shape: 'line', drawStrategy: 'single-click', color: fromRGBA(31, 183, 107, 200) })
    },
    {
      title: 'Circle',
      tool: createTool({ renderer, shape: 'circle', drawStrategy: 'single-click', color: fromRGBA(255, 0, 0, 200) })
    },
    {
      title: 'Ellipse',
      tool: createTool({ renderer, shape: 'ellipse', drawStrategy: 'single-click', color: fromRGBA(183, 31, 183, 200) })
    },
    {
      title: 'Bezier Curve',
      tool: createTool({
        renderer,
        preview: true,
        shape: 'bezier-curve',
        drawStrategy: 'multi-click',
        color: fromRGBA(0, 0, 255, 200)
      }),
      getOptions: () => {
        return { steps: Number(window.prompt('Set order', '4')) }
      }
    },
    {
      title: 'Bezier Surface',
      tool: createTool({
        renderer,
        preview: false,
        shape: 'bezier-surface',
        drawStrategy: 'multi-click',
        color: fromRGBA(83, 83, 212, 200)
      }),
      getOptions: () => {
        const rows = Number(window.prompt('Rows', '4'))
        const columns = Number(window.prompt('Columns', '4'))
        return {
          rows,
          columns,
          steps: rows * columns
        }
      }
    },
    {
      title: 'Smooth Polyline',
      tool: createTool({
        renderer,
        preview: true,
        shape: 'smooth-polyline',
        drawStrategy: 'multi-click',
        color: fromRGBA(235, 19, 44, 200)
      }),
      getOptions: () => {
        return { steps: Number(window.prompt('Set order', '4')) }
      }
    },
    {
      title: 'Gradient Fill',
      tool: createTool({
        renderer,
        shape: 'fill',
        drawStrategy: 'single-click',
        color: fromRGBA(183, 31, 183, 200),
        preview: false,
        gradient: true
      })
    },
    {
      title: 'Line Clip',
      tool: createTool({
        renderer,
        shape: 'line-clip',
        drawStrategy: 'multi-click',
        color: fromRGBA(183, 31, 183, 200),
        preview: true,
        steps: 3
      })
    },
    {
      title: 'Translate',
      tool: createTool({
        renderer,
        shape: 'transform2d',
        drawStrategy: 'single-click',
        color: fromRGBA(183, 31, 183, 200),
        transform: 'translate'
      })
    },
    {
      title: 'Rotate',
      tool: createTool({
        renderer,
        shape: 'transform2d',
        drawStrategy: 'single-click',
        color: fromRGBA(183, 31, 183, 200),
        transform: 'rotate'
      })
    },
    {
      title: 'Transform 3D Demo',
      tool: createTool({
        renderer,
        shape: 'transform3d',
        drawStrategy: 'multi-click',
        color: fromRGBA(183, 31, 183, 200),
        steps: 1
      })
    }
  ]

  const actions = [
    {
      title: 'Undo',
      action: renderer.undo
    },
    {
      title: 'Clear',
      action: renderer.clear
    },
    {
      title: 'Redo',
      action: renderer.redo
    }
  ]

  const toolBtns = tools.map(x => {
    const btn = document.createElement('button')
    btn.setAttribute('class', 'mg-tools__btn')
    btn.innerText = x.title
    btn.onclick = () => {
      const prev = document.querySelectorAll('.mg-tools__btn_active')[0]
      if (prev) prev.classList.toggle('mg-tools__btn_active')
      btn.classList.add('mg-tools__btn_active')
      if (currentTool) currentTool.off()
      currentTool = x.tool
      x.getOptions ? currentTool.on(x.getOptions()) : currentTool.on()
    }
    return btn
  })

  const toolBtnsContainer = document.createElement('div')
  toolBtnsContainer.setAttribute('class', 'mg-tools')
  toolBtns.forEach(x => toolBtnsContainer.appendChild(x))

  const actionBtns = actions.map(x => {
    const btn = document.createElement('button')
    btn.setAttribute('class', 'mg-actions__btn')
    btn.innerText = x.title
    btn.onclick = x.action
    return btn
  })

  const actionBtnsContainer = document.createElement('div')
  actionBtnsContainer.setAttribute('class', 'mg-actions')
  actionBtns.forEach(x => actionBtnsContainer.appendChild(x))

  appContainer.appendChild(toolBtnsContainer)
  appContainer.appendChild(canvas)
  appContainer.appendChild(actionBtnsContainer)
  app.appendChild(appContainer)
}
