import { Title, Container, Stack, Tabs, Center, Loader, Text, Box, TextInput, Textarea, Group, Button, Grid, Flex } from '@mantine/core'
import { useParams } from '@tanstack/react-router'
import { useGetApiDocumentsId, usePutApiDocumentsId } from '../api/generated/documents/documents'
import { useEffect, useState } from 'react'
import { AXIOS_INSTANCE } from '../api/axios-instance'
import { notifications } from '@mantine/notifications'

export function DocumentDetail() {
  const { documentId } = useParams({ from: '/documents/$documentId' })
  const { data, isLoading, isError, error, refetch } = useGetApiDocumentsId(documentId)
  const [blobUrl, setBlobUrl] = useState<string | null>(null)
  const [fileLoading, setFileLoading] = useState(false)
  const [fileError, setFileError] = useState<string | null>(null)

  // Edit mode state
  const [isEditMode, setIsEditMode] = useState(false)
  const [editTitle, setEditTitle] = useState('')
  const [editDescription, setEditDescription] = useState('')

  // Update mutation
  const { mutateAsync: updateDocument, isPending: isUpdating } = usePutApiDocumentsId()

  const selectedVersion = data?.selectedVersion
  const isPdf = selectedVersion?.mimeType === 'application/pdf'

  // Handler functions
  const handleEdit = () => {
    if (data) {
      setEditTitle(data.title)
      setEditDescription(data.description || '')
      setIsEditMode(true)
    }
  }

  const handleDiscard = () => {
    setIsEditMode(false)
    setEditTitle('')
    setEditDescription('')
  }

  const handleSave = async () => {
    try {
      await updateDocument({
        id: documentId,
        data: {
          title: editTitle,
          description: editDescription || null,
        },
      })

      notifications.show({
        title: 'Success',
        message: 'Document updated successfully',
        color: 'green',
      })

      setIsEditMode(false)
      refetch()
    } catch (err) {
      notifications.show({
        title: 'Error',
        message: err instanceof Error ? err.message : 'Failed to update document',
        color: 'red',
      })
    }
  }

  // Fetch the file with authentication when the selected version changes
  useEffect(() => {
    if (!selectedVersion || !isPdf) {
      return
    }

    let objectUrl: string | null = null

    const fetchFile = async () => {
      setFileLoading(true)
      setFileError(null)

      try {
        const response = await AXIOS_INSTANCE.get(
          `/api/documents/versions/${selectedVersion.id}/file`,
          {
            responseType: 'blob',
          }
        )

        // Create a blob URL from the response
        const blob = new Blob([response.data], { type: selectedVersion.mimeType })
        const url = URL.createObjectURL(blob)
        objectUrl = url
        setBlobUrl(url)
      } catch (err) {
        setFileError(err instanceof Error ? err.message : 'Failed to load file')
      } finally {
        setFileLoading(false)
      }
    }

    fetchFile()

    // Cleanup blob URL when component unmounts or version changes
    return () => {
      if (objectUrl) {
        URL.revokeObjectURL(objectUrl)
      }
    }
  }, [selectedVersion, isPdf])

  if (isLoading) {
    return (
      <Container fluid py="xs">
        <Center p="xl">
          <Loader />
        </Center>
      </Container>
    )
  }

  if (isError) {
    return (
      <Container fluid py="xs">
        <Text c="red">Failed to load document: {error?.message || 'Unknown error'}</Text>
      </Container>
    )
  }

  if (!data) {
    return (
      <Container fluid py="xs">
        <Text c="dimmed">Document not found</Text>
      </Container>
    )
  }

  // Helper function to render preview content
  const renderPreviewContent = () => {
    if (isPdf) {
      return (
        <Box style={{ width: '100%', height: '100%' }}>
          {fileLoading ? (
            <Center p="xl">
              <Loader />
            </Center>
          ) : fileError ? (
            <Center p="xl">
              <Text c="red">{fileError}</Text>
            </Center>
          ) : blobUrl ? (
            <iframe
              src={blobUrl}
              style={{
                width: '100%',
                height: '100%',
                border: 'none',
                overflow: 'hidden'
              }}
              title={`Preview of ${data.title}`}
            />
          ) : null}
        </Box>
      )
    }

    return (
      <Center p="xl">
        <Stack align="center" gap="md">
          <Text c="dimmed" size="lg">
            Preview not supported for this file type
          </Text>
          <Text c="dimmed" size="sm">
            MIME Type: {selectedVersion?.mimeType}
          </Text>
        </Stack>
      </Center>
    )
  }

  // Helper function to render details content
  const renderDetailsContent = () => {
    if (isEditMode) {
      return (
        <Stack gap="md">
          <TextInput
            label="Title"
            value={editTitle}
            onChange={(e) => setEditTitle(e.target.value)}
            required
            placeholder="Enter document title"
            error={editTitle.trim() === '' ? 'Title cannot be empty' : undefined}
          />
          <Textarea
            label="Description"
            value={editDescription}
            onChange={(e) => setEditDescription(e.target.value)}
            placeholder="Enter document description (optional)"
            rows={4}
          />
          <Group>
            <Button
              onClick={handleSave}
              loading={isUpdating}
              disabled={editTitle.trim() === ''}
            >
              Save
            </Button>
            <Button variant="outline" onClick={handleDiscard} disabled={isUpdating}>
              Discard
            </Button>
          </Group>
        </Stack>
      )
    }

    return (
      <Stack gap="md">
        <div>
          <Text size="sm" fw={500} c="dimmed">Title</Text>
          <Text size="md">{data.title}</Text>
        </div>
        <div>
          <Text size="sm" fw={500} c="dimmed">Description</Text>
          <Text size="md">{data.description || 'No description'}</Text>
        </div>
        <Group>
          <Button onClick={handleEdit}>Edit</Button>
        </Group>
      </Stack>
    )
  }

  return (
    <Stack gap="lg" h="calc(100vh - var(--app-shell-header-height, 0px) - var(--app-shell-footer-height, 0px) - 2 * var(--mantine-spacing-md, 0px))" style={{overflow: "hidden"}}>
      <Title order={1}>{data.title}</Title>

      {/* Mobile Layout: Single merged tab group */}
      <Flex direction="column" hiddenFrom="md" style={{ flexGrow: 1 }}>
        <Tabs defaultValue="preview" style={{ flexGrow: 1 }}>
          <Tabs.List>
            <Tabs.Tab value="preview">Preview</Tabs.Tab>
            <Tabs.Tab value="details">Details</Tabs.Tab>
          </Tabs.List>

          <Tabs.Panel value="preview" pt="md" h="100%">
            {renderPreviewContent()}
          </Tabs.Panel>

          <Tabs.Panel value="details" pt="md">
            {renderDetailsContent()}
          </Tabs.Panel>
        </Tabs>
      </Flex>

      {/* Desktop Layout: Side-by-side preview and details */}
      <Flex visibleFrom="md" gap="lg" style={{ flexGrow: 1 }}>
        <Box style={{ flexGrow: 3 }}>
          {renderPreviewContent()}
        </Box>
        <Box style={{ flexGrow: 1 }}>
          <Tabs defaultValue="details">
            <Tabs.List>
              <Tabs.Tab value="details">Details</Tabs.Tab>
            </Tabs.List>

            <Tabs.Panel value="details" pt="md">
              {renderDetailsContent()}
            </Tabs.Panel>
          </Tabs>
        </Box>
      </Flex>
    </Stack>
  )
}
