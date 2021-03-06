﻿!function() {
    "use strict";
    var e = tinymce.util.Tools.resolve("tinymce.PluginManager"),
        t = tinymce.util.Tools.resolve("tinymce.Env"),
        r = tinymce.util.Tools.resolve("tinymce.util.Tools"),
        i = function(e) { return e.getParam("media_scripts") },
        a = function(e) { return e.getParam("audio_template_callback") },
        o = function(e) { return e.getParam("video_template_callback") },
        n = function(e) { return e.getParam("media_live_embeds", !0) },
        c = function(e) { return e.getParam("media_filter_html", !0) },
        s = function(e) { return e.getParam("media_url_resolver") },
        u = function(e) { return e.getParam("media_alt_source", !0) },
        l = function(e) { return e.getParam("media_poster", !0) },
        m = function(e) { return e.getParam("media_dimensions", !0) },
        d = tinymce.util.Tools.resolve("tinymce.html.SaxParser"),
        h = tinymce.util.Tools.resolve("tinymce.dom.DOMUtils"),
        p = function(e, t) { if (e) for (var r = 0; r < e.length; r++) if (-1 !== t.indexOf(e[r].filter)) return e[r] },
        f = function(e) { return function(t) { return t ? t.style[e].replace(/px$/, "") : "" } },
        g = function(e) {
            return function(t, r) {
                var i;
                t && (t.style[e] = /^[0-9.]+$/.test(i = r) ? i + "px" : i)
            }
        },
        v =
        {
            getMaxWidth: f("maxWidth"),
            getMaxHeight: f("maxHeight"),
            setMaxWidth: g("maxWidth"),
            setMaxHeight: g("maxHeight")
        },
        w = h.DOM,
        b = function(e) { return w.getAttrib(e, "data-ephox-embed-iri") },
        y = function(e, t) {
            return s = t, u = w.createFragment(s), "" !== b(u.firstChild)
                ? (n = t, c =
                        w.createFragment(n).firstChild,
                    {
                        type: "ephox-embed-iri",
                        source1: b(c),
                        source2: "",
                        poster: "",
                        width: v.getMaxWidth(c),
                        height: v.getMaxHeight(c)
                    })
                : (i = e, a = t, o = {}, d({
                    validate: !1,
                    allow_conditional_comments: !0,
                    special: "script,noscript",
                    start: function(e, t) {
                        if (o.source1 || "param" !== e || (o.source1 = t.map.movie), "iframe" !== e &&
                            "object" !== e &&
                            "embed" !== e &&
                            "video" !== e &&
                            "audio" !== e ||
                            (o.type || (o.type = e), o = r.extend(t.map, o)), "script" === e) {
                            var a = p(i, t.map.src);
                            if (!a) return;
                            o = { type: "script", source1: t.map.src, width: a.width, height: a.height }
                        }
                        "source" === e && (o.source1 ? o.source2 || (o.source2 = t.map.src) : o.source1 = t.map.src),
                            "img" !== e || o.poster || (o.poster = t.map.src)
                    }
                }).parse(a), o.source1 = o.source1 || o.src || o.data, o.source2 = o.source2 || "", o.poster =
                    o.poster || "", o);
            var i, a, o, n, c, s, u
        },
        x = tinymce.util.Tools.resolve("tinymce.util.Promise"),
        j = function(e) {
            var t = {
                mp3: "audio/mpeg",
                wav: "audio/wav",
                mp4: "video/mp4",
                webm: "video/webm",
                ogg: "video/ogg",
                swf: "application/x-shockwave-flash"
            }[e.toLowerCase().split(".").pop()];
            return t || ""
        },
        M = tinymce.util.Tools.resolve("tinymce.html.Writer"),
        _ = tinymce.util.Tools.resolve("tinymce.html.Schema"),
        C = h.DOM,
        S = function(e, t) {
            var r, i, a, o;
            for (r in t)
                if (a = "" + t[r], e.map[r])
                    for (i = e.length; i--;)
                        (o = e[i]).name === r && (a ? (e.map[r] = a, o.value = a) : (delete e.map[r], e.splice(i, 1)));
                else a && (e.push({ name: r, value: a }), e.map[r] = a)
        },
        k = function(e, t) {
            var r, i, a = C.createFragment(e).firstChild;
            return v.setMaxWidth(a, t.width), v.setMaxHeight(a, t.height), r = a.outerHTML, i =
                M(), d(i).parse(r), i.getContent()
        },
        A = function(e, t, r) {
            return u = e, l = C.createFragment(u), "" !== C.getAttrib(l.firstChild, "data-ephox-embed-iri")
                ? k(e, t)
                : (i = e, a = t, o = r, c = M(), s = 0, d({
                        validate: !1,
                        allow_conditional_comments: !0,
                        special: "script,noscript",
                        comment: function(e) { c.comment(e) },
                        cdata: function(e) { c.cdata(e) },
                        text: function(e, t) { c.text(e, t) },
                        start: function(e, t, r) {
                            switch (e) {
                            case"video":
                            case"object":
                            case"embed":
                            case"img":
                            case"iframe":
                                a.height !== undefined &&
                                    a.width !== undefined &&
                                    S(t, { width: a.width, height: a.height })
                            }
                            if (o)
                                switch (e) {
                                case"video":
                                    S(t, { poster: a.poster, src: "" }), a.source2 && S(t, { src: "" });
                                    break;
                                case"iframe":
                                    S(t, { src: a.source1 });
                                    break;
                                case"source":
                                    if (++s <= 2 &&
                                    (S(t,
                                        { src: a["source" + s], type: a["source" + s + "mime"] }), !a["source" + s]))
                                        return;
                                    break;
                                case"img":
                                    if (!a.poster) return;
                                    n = !0
                                }
                            c.start(e, t, r)
                        },
                        end: function(e) {
                            if ("video" === e && o)
                                for (var t = 1; t <= 2; t++)
                                    if (a["source" + t]) {
                                        var r = [];
                                        r.map = {}, s < t &&
                                        (S(r, { src: a["source" + t], type: a["source" + t + "mime"] }), c.start(
                                            "source",
                                            r,
                                            !0))
                                    }
                            if (a.poster && "object" === e && o && !n) {
                                var i = [];
                                i.map = {}, S(i, { src: a.poster, width: a.width, height: a.height }), c.start("img",
                                    i,
                                    !0)
                            }
                            c.end(e)
                        }
                    },
                    _({})).parse(i), c.getContent());
            var i, a, o, n, c, s, u, l
        },
        F = [
            {
                regex: /youtu\.be\/([\w\-.]+)/,
                type: "iframe",
                w: 560,
                h: 314,
                url: "//www.youtube.com/embed/$1",
                allowFullscreen: !0
            },
            {
                regex: /youtube\.com(.+)v=([^&]+)/,
                type: "iframe",
                w: 560,
                h: 314,
                url: "//www.youtube.com/embed/$2",
                allowFullscreen: !0
            },
            {
                regex: /youtube.com\/embed\/([a-z0-9\-_]+(?:\?.+)?)/i,
                type: "iframe",
                w: 560,
                h: 314,
                url: "//www.youtube.com/embed/$1",
                allowFullscreen: !0
            },
            {
                regex: /vimeo\.com\/([0-9]+)/,
                type: "iframe",
                w: 425,
                h: 350,
                url: "//player.vimeo.com/video/$1?title=0&byline=0&portrait=0&color=8dc7dc",
                allowfullscreen: !0
            },
            {
                regex: /vimeo\.com\/(.*)\/([0-9]+)/,
                type: "iframe",
                w: 425,
                h: 350,
                url: "//player.vimeo.com/video/$2?title=0&amp;byline=0",
                allowfullscreen: !0
            },
            {
                regex: /maps\.google\.([a-z]{2,3})\/maps\/(.+)msid=(.+)/,
                type: "iframe",
                w: 425,
                h: 350,
                url: '//maps.google.com/maps/ms?msid=$2&output=embed"',
                allowFullscreen: !1
            },
            {
                regex: /dailymotion\.com\/video\/([^_]+)/,
                type: "iframe",
                w: 480,
                h: 270,
                url: "//www.dailymotion.com/embed/video/$1",
                allowFullscreen: !0
            },
            {
                regex: /dai\.ly\/([^_]+)/,
                type: "iframe",
                w: 480,
                h: 270,
                url: "//www.dailymotion.com/embed/video/$1",
                allowFullscreen: !0
            }
        ],
        N = function(e, t) {
            var n = r.extend({}, t);
            if (!n.source1 && (r.extend(n, y(i(e), n.embed)), !n.source1)) return"";
            if (n.source2 || (n.source2 = ""), n.poster || (n.poster = ""), n.source1 =
                e.convertURL(n.source1, "source"), n.source2 = e.convertURL(n.source2, "source"), n.source1mime =
                j(n.source1), n.source2mime = j(n.source2), n.poster = e.convertURL(n.poster, "poster"), r.each(F,
                function(e) {
                    var t, r, i = e.regex.exec(n.source1);
                    if (i) {
                        for (r = e.url, t = 0; i[t]; t++) r = r.replace("$" + t, function() { return i[t] });
                        n.source1 = r, n.type = e.type, n.allowFullscreen = e.allowFullscreen, n.width =
                            n.width || e.w, n.height = n.height || e.h
                    }
                }), n.embed) return A(n.embed, n, !0);
            var c = p(i(e), n.source1);
            c && (n.type = "script", n.width = c.width, n.height = c.height);
            var s, u, l, m, d, h, f, g, v = a(e), w = o(e);
            return n.width = n.width || 300, n.height =
                n.height || 150, r.each(n, function(t, r) { n[r] = e.dom.encode(t) }), "iframe" === n.type
                ? (g =
                        (f = n).allowFullscreen ? ' allowFullscreen="1"' : "",
                    '<iframe src="' +
                        f.source1 +
                        '" width="' +
                        f.width +
                        '" height="' +
                        f.height +
                        '"' +
                        g +
                        "></iframe>")
                : "application/x-shockwave-flash" === n.source1mime
                ? (h =
                        '<object data="' +
                        (d = n).source1 +
                        '" width="' +
                        d.width +
                        '" height="' +
                        d.height +
                        '" type="application/x-shockwave-flash">',
                    d.poster &&
                        (h += '<img src="' + d.poster + '" width="' + d.width + '" height="' + d.height + '" />'), h +=
                        "</object>")
                : -1 !== n.source1mime.indexOf("audio")
                ? (l =
                    n, (m = v)
                    ? m(l)
                    : '<audio controls="controls" src="' +
                    l.source1 +
                    '">' +
                    (l.source2
                        ? '\n<source src="' +
                        l.source2 +
                        '"' +
                        (l.source2mime ? ' type="' + l.source2mime + '"' : "") +
                        " />\n"
                        : "") +
                    "</audio>")
                : "script" === n.type
                ? '<script src="' + n.source1 + '"><\/script>'
                : (s = n, (u = w)
                    ? u(s)
                    : '<video width="' +
                    s.width +
                    '" height="' +
                    s.height +
                    '"' +
                    (s.poster ? ' poster="' + s.poster + '"' : "") +
                    ' controls="controls">\n<source src="' +
                    s.source1 +
                    '"' +
                    (s.source1mime ? ' type="' + s.source1mime + '"' : "") +
                    " />\n" +
                    (s.source2
                        ? '<source src="' +
                        s.source2 +
                        '"' +
                        (s.source2mime ? ' type="' + s.source2mime + '"' : "") +
                        " />\n"
                        : "") +
                    "</video>")
        },
        O = {},
        P = function(e) { return function(t) { return N(e, t) } },
        T = function(e, t) {
            var r, i, a, o, n, c = s(e);
            return c
                ? (a = t, o = P(e), n = c, new x(function(e, t) {
                    var r = function(t) {
                        return t.html && (O[a.source1] = t), e({ url: a.source1, html: t.html ? t.html : o(a) })
                    };
                    O[a.source1] ? r(O[a.source1]) : n({ url: a.source1 }, r, t)
                }))
                : (r = t, i = P(e), new x(function(e) { e({ html: i(r), url: r.source1 }) }))
        },
        z = function(e) { return O.hasOwnProperty(e) },
        $ = function(e, t) { e.state.set("oldVal", e.value()), t.state.set("oldVal", t.value()) },
        L = function(e, t) {
            var r = e.find("#width")[0], i = e.find("#height")[0], a = e.find("#constrain")[0];
            r && i && a && t(r, i, a.checked())
        },
        H = function(e, t, r) {
            var i = e.state.get("oldVal"), a = t.state.get("oldVal"), o = e.value(), n = t.value();
            r &&
                i &&
                a &&
                o &&
                n &&
                (o !== i
                    ? (n = Math.round(o / i * n), isNaN(n) || t.value(n))
                    : (o = Math.round(n / a * o), isNaN(o) || e.value(o))), $(e, t)
        },
        W = function(e) { L(e, H) },
        J = function(e) {
            var t = function() { e(function(e) { W(e) }) };
            return{
                type: "container",
                label: "Dimensions",
                layout: "flex",
                align: "center",
                spacing: 5,
                items: [
                    { name: "width", type: "textbox", maxLength: 5, size: 5, onchange: t, ariaLabel: "Width" },
                    { type: "label", text: "x" },
                    { name: "height", type: "textbox", maxLength: 5, size: 5, onchange: t, ariaLabel: "Height" },
                    { name: "constrain", type: "checkbox", checked: !0, text: "Constrain proportions" }
                ]
            }
        },
        R = function(e) { L(e, $) },
        D = W,
        E = t.ie && t.ie <= 8 ? "onChange" : "onInput",
        I = function(e) {
            return function(t) {
                var r = t && t.msg ? "Media embed handler error: " + t.msg : "Media embed handler threw unknown error.";
                e.notificationManager.open({ type: "error", text: r })
            }
        },
        U = function(e, t) {
            return function(a) {
                var o = a.html, n = e.find("#embed")[0], c = r.extend(y(i(t), o), { source1: a.url });
                e.fromJSON(c), n && (n.value(o), D(e))
            }
        },
        V = function(e, t) {
            var r = e.dom.select("img[data-mce-object]");
            e.insertContent(t), function(e, t) {
                var r, i, a = e.dom.select("img[data-mce-object]");
                for (r = 0; r < t.length; r++) for (i = a.length - 1; i >= 0; i--) t[r] === a[i] && a.splice(i, 1);
                e.selection.select(a[0])
            }(e, r), e.nodeChanged()
        },
        B = function(e) {
            var t,
                a,
                o,
                n,
                c,
                s = [
                    {
                        name: "source1",
                        type: "filepicker",
                        filetype: "media",
                        size: 40,
                        autofocus: !0,
                        label: "Source",
                        onpaste: function() {
                            setTimeout(function() { T(e, t.toJSON()).then(U(t, e))["catch"](I(e)) }, 1)
                        },
                        onchange: function(i) {
                            var a, o;
                            T(e, t.toJSON()).then(U(t, e))["catch"](I(e)), a = t, o =
                                i.meta, r.each(o, function(e, t) { a.find("#" + t).value(e) })
                        },
                        onbeforecall: function(e) { e.meta = t.toJSON() }
                    }
                ],
                d = [];
            if (u(e) &&
                    d.push({
                        name: "source2",
                        type: "filepicker",
                        filetype: "media",
                        size: 40,
                        label: "Alternative source"
                    }),
                l(e) &&
                    d.push({
                        name: "poster",
                        type: "filepicker",
                        filetype: "image",
                        size: 40,
                        label: "Poster"
                    }), m(e)) {
                var h = J(function(e) { e(t), a = t.toJSON(), t.find("#embed").value(A(a.embed, a)) });
                s.push(h)
            }
            n = (o = e).selection.getNode(), c = n.getAttribute("data-ephox-embed-iri"), a = c
                ? { source1: c, "data-ephox-embed-iri": c, width: v.getMaxWidth(n), height: v.getMaxHeight(n) }
                : n.getAttribute("data-mce-object")
                ? y(i(o), o.serializer.serialize(n, { selection: !0 }))
                : {};
            var p = {
                id: "mcemediasource",
                type: "textbox",
                flex: 1,
                name: "embed",
                value: function(e) {
                    var t = e.selection.getNode();
                    if (t.getAttribute("data-mce-object") || t.getAttribute("data-ephox-embed-iri"))
                        return e.selection.getContent()
                }(e),
                multiline: !0,
                rows: 5,
                label: "Source"
            };
            p[E] = function() { a = r.extend({}, y(i(e), this.value())), this.parent().parent().fromJSON(a) };
            var f = [
                { title: "General", type: "form", items: s },
                {
                    title: "Embed",
                    type: "container",
                    layout: "flex",
                    direction: "column",
                    align: "stretch",
                    padding: 10,
                    spacing: 10,
                    items: [{ type: "label", text: "Paste your embed code below:", forId: "mcemediasource" }, p]
                }
            ];
            d.length > 0 && f.push({ title: "Advanced", type: "form", items: d }), t = e.windowManager.open({
                title: "Insert/edit media",
                data: a,
                bodyType: "tabpanel",
                body: f,
                onSubmit: function() {
                    var r, i;
                    D(t), r = e, (i = t.toJSON()).embed = A(i.embed, i), i.embed && z(i.source1)
                        ? V(r, i.embed)
                        : T(r, i).then(function(e) { V(r, e.html) })["catch"](I(r))
                }
            }), R(t)
        },
        G = function(e) { return{ showDialog: function() { B(e) } } },
        q = function(e) { e.addCommand("mceMedia", function() { B(e) }) },
        K = tinymce.util.Tools.resolve("tinymce.html.Node"),
        Q = function(e, t) {
            if (!1 === c(e)) return t;
            var r, i = M();
            return d({
                    validate: !1,
                    allow_conditional_comments: !1,
                    special: "script,noscript",
                    comment: function(e) { i.comment(e) },
                    cdata: function(e) { i.cdata(e) },
                    text: function(e, t) { i.text(e, t) },
                    start: function(t, a, o) {
                        if (r = !0, "script" !== t && "noscript" !== t) {
                            for (var n = 0; n < a.length; n++) {
                                if (0 === a[n].name.indexOf("on")) return;
                                "style" === a[n].name &&
                                    (a[n].value = e.dom.serializeStyle(e.dom.parseStyle(a[n].value), t))
                            }
                            i.start(t, a, o), r = !1
                        }
                    },
                    end: function(e) { r || i.end(e) }
                },
                _({})).parse(t), i.getContent()
        },
        X = function(e, r) {
            var i, a = r.name;
            return(i = new K("img", 1)).shortEnded = !0, Z(e, r, i), i.attr({
                width: r.attr("width") || "300",
                height: r.attr("height") || ("audio" === a ? "30" : "150"),
                style: r.attr("style"),
                src: t.transparentSrc,
                "data-mce-object": a,
                "class": "mce-object mce-object-" + a
            }), i
        },
        Y = function(e, t) {
            var r, i, a, o = t.name;
            return(r = new K("span", 1)).attr({
                contentEditable: "false",
                style: t.attr("style"),
                "data-mce-object": o,
                "class": "mce-preview-object mce-object-" + o
            }), Z(e, t, r), (i = new K(o, 1)).attr({
                src: t.attr("src"),
                allowfullscreen: t.attr("allowfullscreen"),
                style: t.attr("style"),
                "class": t.attr("class"),
                width: t.attr("width"),
                height: t.attr("height"),
                frameborder: "0"
            }), (a = new K("span", 1)).attr("class", "mce-shim"), r.append(i), r.append(a), r
        },
        Z = function(e, t, r) {
            var i, a, o, n, c;
            for (n = (o = t.attributes).length; n--;)
                i = o[n].name, a = o[n].value, "width" !== i &&
                    "height" !== i &&
                    "style" !== i &&
                    ("data" !== i && "src" !== i || (a = e.convertURL(a, i)), r.attr("data-mce-p-" + i, a));
            (c = t.firstChild && t.firstChild.value) && (r.attr("data-mce-html", escape(Q(e, c))), r.firstChild = null)
        },
        ee = function(e) {
            for (; e = e.parent;) if (e.attr("data-ephox-embed-iri")) return!0;
            return!1
        },
        te = function(e) {
            return function(r) {
                for (var a, o, c = r.length; c--;)
                    (a = r[c]).parent &&
                    (a.parent.attr("data-mce-object") ||
                        ("script" !== a.name || (o = p(i(e), a.attr("src")))) &&
                        (o &&
                        (o.width && a.attr("width", o.width.toString()), o.height &&
                            a.attr("height", o.height.toString())), "iframe" === a.name && n(e) && t.ceFalse
                            ? ee(a) || a.replace(Y(e, a))
                            : ee(a) || a.replace(X(e, a))))
            }
        },
        re = function(e) {
            e.on("preInit",
                function() {
                    var t = e.schema.getSpecialElements();
                    r.each("video audio iframe object".split(" "),
                        function(e) { t[e] = new RegExp("</" + e + "[^>]*>", "gi") });
                    var i = e.schema.getBoolAttrs();
                    r.each("webkitallowfullscreen mozallowfullscreen allowfullscreen".split(" "),
                            function(e) { i[e] = {} }), e.parser.addNodeFilter("iframe,video,audio,object,embed,script",
                            te(e)),
                        e.serializer.addAttributeFilter("data-mce-object",
                            function(t, r) {
                                for (var i, a, o, n, c, s, u, l, m = t.length; m--;)
                                    if ((i = t[m]).parent) {
                                        for (u = i.attr(r), a =
                                                new K(u, 1), "audio" !== u &&
                                                "script" !== u &&
                                                ((l = i.attr("class")) && -1 !== l.indexOf("mce-preview-object")
                                                    ? a.attr({
                                                        width: i.firstChild.attr("width"),
                                                        height: i.firstChild.attr("height")
                                                    })
                                                    : a.attr({ width: i.attr("width"), height: i.attr("height") })), a
                                                .attr({
                                                    style: i.attr("style")
                                                }), o = (n = i.attributes).length;
                                            o--;
                                        ) {
                                            var d = n[o].name;
                                            0 === d.indexOf("data-mce-p-") && a.attr(d.substr(11), n[o].value)
                                        }
                                        "script" === u && a.attr("type", "text/javascript"), (c =
                                                i.attr("data-mce-html")) &&
                                            ((s = new K("#text", 3)).raw = !0, s.value =
                                                Q(e, unescape(c)), a.append(s)), i
                                            .replace(a)
                                    }
                            })
                }), e.on("setContent",
                function() {
                    e.$("span.mce-preview-object").each(function(t, r) {
                        var i = e.$(r);
                        0 === i.find("span.mce-shim", r).length && i.append('<span class="mce-shim"></span>')
                    })
                })
        },
        ie = function(e) {
            e.on("ResolveName",
                function(e) {
                    var t;
                    1 === e.target.nodeType && (t = e.target.getAttribute("data-mce-object")) && (e.name = t)
                })
        },
        ae = function(e) {
            e.on("click keyup",
                function() {
                    var t = e.selection.getNode();
                    t &&
                        e.dom.hasClass(t, "mce-preview-object") &&
                        e.dom.getAttrib(t, "data-mce-selected") &&
                        t.setAttribute("data-mce-selected", "2")
                }), e.on("ObjectSelected",
                function(e) {
                    var t = e.target.getAttribute("data-mce-object");
                    "audio" !== t && "script" !== t || e.preventDefault()
                }), e.on("objectResized",
                function(e) {
                    var t, r = e.target;
                    r.getAttribute("data-mce-object") &&
                        (t = r.getAttribute("data-mce-html")) &&
                        (t = unescape(t), r.setAttribute("data-mce-html",
                            escape(A(t, { width: e.width, height: e.height }))))
                })
        },
        oe = function(e) {
            e.addButton("media",
                {
                    tooltip: "Insert/edit media",
                    cmd: "mceMedia",
                    stateSelector: ["img[data-mce-object]", "span[data-mce-object]", "div[data-ephox-embed-iri]"]
                }), e.addMenuItem("media",
                { icon: "media", text: "Media", cmd: "mceMedia", context: "insert", prependToContext: !0 })
        };
    e.add("media", function(e) { return q(e), oe(e), ie(e), re(e), ae(e), G(e) })
}();